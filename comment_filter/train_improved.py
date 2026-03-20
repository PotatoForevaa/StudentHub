import pandas as pd
import torch
import numpy as np
from torch.utils.data import Dataset
from torch import nn
from transformers import (
    AutoTokenizer, 
    AutoModelForSequenceClassification, 
    Trainer, 
    TrainingArguments,
    EarlyStoppingCallback,
    get_linear_schedule_with_warmup
)
from sklearn.model_selection import train_test_split
from sklearn.utils.class_weight import compute_class_weight
from sklearn.metrics import accuracy_score, f1_score, precision_score, recall_score, confusion_matrix
import re
import logging
from pathlib import Path
import json
from datetime import datetime

def load_hf_parquet_dataset(train_path, test_path, sample_size=50000):
    # Загружаем parquet файлы
    train_df = pd.read_parquet(train_path)
    test_df = pd.read_parquet(test_path)
    
    # Объединяем
    df = pd.concat([train_df, test_df], ignore_index=True)
    
    # Переименуем колонки для совместимости с остальным кодом
    df = df.rename(columns={'text': 'comment', 'label': 'toxic'})
    
    # Инвертируем лейблы: 1 -> не токсик, 0 -> токсик
    df['toxic'] = 1 - df['toxic']
    
    # Очищаем текст
    df['comment'] = df['comment'].apply(clean_text)
    
    # Убираем пустые комментарии
    df = df[df['comment'].str.len() > 0]
    
    # Ограничиваем размер
    if sample_size is not None:
        df = df.sample(n=min(sample_size, len(df)), random_state=42).reset_index(drop=True)
    
    logger.info(f"Dataset size after cleaning and sampling: {len(df)}")
    return df


# Setup logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# Configuration
device = "cuda" if torch.cuda.is_available() else "cpu"
MODEL_NAME = "DeepPavlov/rubert-base-cased"
MAX_LENGTH = 128
BATCH_SIZE = 16
LEARNING_RATE = 2e-5
NUM_EPOCHS = 3
WARMUP_STEPS = 500
EARLY_STOPPING_PATIENCE = 2

def clean_text(text):
    """Clean and normalize text data"""
    if pd.isna(text):
        return ""
    
    # Convert to string
    text = str(text)
    
    # Remove HTML tags
    text = re.sub(r'<[^>]+>', '', text)
    
    # Remove URLs
    text = re.sub(r'http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\\(\\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+', '', text)
    
    # Remove extra whitespace
    text = re.sub(r'\s+', ' ', text).strip()
    
    # Remove special characters but keep Cyrillic and basic punctuation
    text = re.sub(r'[^\w\sа-яА-ЯёЁ.,!?-]', ' ', text)
    
    return text

def load_and_preprocess_data(csv_path):
    """Load and preprocess the dataset"""
    logger.info(f"Loading data from {csv_path}")
    
    # Load data
    data = load_hf_parquet_dataset("train-00000-of-00001.parquet", "test-00000-of-00001.parquet", sample_size=50000)
    
    # Basic validation
    if 'comment' not in data.columns or 'toxic' not in data.columns:
        raise ValueError("CSV must contain 'comment' and 'toxic' columns")
    
    # Remove rows with missing values
    initial_count = len(data)
    data = data.dropna(subset=['comment', 'toxic'])
    logger.info(f"Removed {initial_count - len(data)} rows with missing values")
    
    # Clean text
    logger.info("Cleaning text data...")
    data['comment'] = data['comment'].apply(clean_text)
    
    # Remove empty comments
    data = data[data['comment'].str.len() > 0]
    logger.info(f"Removed empty comments, remaining: {len(data)}")
    
    # Validate labels
    unique_labels = data['toxic'].unique()
    if not set(unique_labels).issubset({0, 1}):
        raise ValueError(f"Labels must be 0 or 1, found: {unique_labels}")
    
    # Class distribution
    class_dist = data['toxic'].value_counts()
    logger.info(f"Class distribution: {dict(class_dist)}")
    
    return data

def compute_class_weights(y):
    """Compute class weights for imbalanced datasets"""
    classes = np.unique(y)
    weights = compute_class_weight('balanced', classes=classes, y=y)
    class_weights = torch.tensor(weights, dtype=torch.float).to(device)
    logger.info(f"Computed class weights: {class_weights.cpu().numpy()}")
    return class_weights

class ToxicDataset(Dataset):
    def __init__(self, texts, labels, tokenizer, max_length=128):
        self.texts = texts
        self.labels = labels
        self.tokenizer = tokenizer
        self.max_length = max_length

    def __len__(self):
        return len(self.labels)

    def __getitem__(self, idx):
        text = str(self.texts[idx])
        label = self.labels[idx]
        
        encoding = self.tokenizer(
            text,
            truncation=True,
            padding='max_length',
            max_length=self.max_length,
            return_tensors='pt'
        )
        
        return {
            'input_ids': encoding['input_ids'].flatten(),
            'attention_mask': encoding['attention_mask'].flatten(),
            'labels': torch.tensor(label, dtype=torch.long)
        }

def compute_detailed_metrics(eval_pred):
    """Compute comprehensive evaluation metrics"""
    logits, labels = eval_pred
    predictions = np.argmax(logits, axis=-1)
    
    accuracy = accuracy_score(labels, predictions)
    f1 = f1_score(labels, predictions, average='binary')
    precision = precision_score(labels, predictions, average='binary')
    recall = recall_score(labels, predictions, average='binary')
    conf_matrix = confusion_matrix(labels, predictions)
    
    return {
        'accuracy': accuracy,
        'f1': f1,
        'precision': precision,
        'recall': recall,
        'confusion_matrix': conf_matrix.tolist()
    }

class WeightedTrainer(Trainer):
    def __init__(self, class_weights, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.class_weights = class_weights

    def compute_loss(self, model, inputs, return_outputs=False, **kwargs):
        labels = inputs.get("labels")
        outputs = model(**inputs)
        logits = outputs.logits
        
        loss_fn = nn.CrossEntropyLoss(weight=self.class_weights)
        loss = loss_fn(logits, labels)
        
        return (loss, outputs) if return_outputs else loss

def create_model_dir():
    """Create a timestamped model directory"""
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    model_dir = Path(f"toxic_model_v{timestamp}")
    model_dir.mkdir(exist_ok=True)
    return model_dir

def save_training_config(model_dir, config):
    """Save training configuration"""
    config_path = model_dir / "training_config.json"
    with open(config_path, 'w') as f:
        json.dump(config, f, indent=2, default=str)
    logger.info(f"Training config saved to {config_path}")

def main():
    # Load and preprocess data
    data = load_and_preprocess_data("toxic_comments.csv")
    
    # Compute class weights
    class_weights = compute_class_weights(data['toxic'].values)
    
    # Split data with stratification
    train_texts, test_texts, train_labels, test_labels = train_test_split(
        data['comment'].tolist(),
        data['toxic'].tolist(),
        test_size=0.2,
        random_state=42,
        stratify=data['toxic']  # Ensure balanced splits
    )
    
    logger.info(f"Training set size: {len(train_texts)}")
    logger.info(f"Test set size: {len(test_texts)}")
    
    # Initialize tokenizer and model
    logger.info("Loading tokenizer and model...")
    tokenizer = AutoTokenizer.from_pretrained(MODEL_NAME)
    model = AutoModelForSequenceClassification.from_pretrained(
        MODEL_NAME,
        num_labels=2
    )
    model.to(device)
    
    # Create datasets
    train_dataset = ToxicDataset(train_texts, train_labels, tokenizer, MAX_LENGTH)
    test_dataset = ToxicDataset(test_texts, test_labels, tokenizer, MAX_LENGTH)
    
    # Create model directory
    model_dir = create_model_dir()
    
    # Training arguments with improvements
    training_args = TrainingArguments(
        output_dir=str(model_dir),
        num_train_epochs=NUM_EPOCHS,
        per_device_train_batch_size=BATCH_SIZE,
        per_device_eval_batch_size=BATCH_SIZE,
        warmup_steps=WARMUP_STEPS,
        weight_decay=0.01,
        logging_dir=str(model_dir / "logs"),
        logging_strategy="steps",
        logging_steps=100,
        eval_strategy="epoch",
        save_strategy="epoch",
        save_total_limit=3,
        load_best_model_at_end=True,
        metric_for_best_model="f1",
        greater_is_better=True,
        fp16=True,
        dataloader_num_workers=4,
        remove_unused_columns=False
    )
    
    # Create trainer with callbacks
    trainer = WeightedTrainer(
        class_weights=class_weights,
        model=model,
        args=training_args,
        train_dataset=train_dataset,
        eval_dataset=test_dataset,
        compute_metrics=compute_detailed_metrics,
        callbacks=[
            EarlyStoppingCallback(early_stopping_patience=EARLY_STOPPING_PATIENCE)
        ]
    )
    
    # Train model
    logger.info("Starting training...")
    train_result = trainer.train()
    
    # Evaluate model
    logger.info("Evaluating model...")
    eval_result = trainer.evaluate()
    
    # Save model and tokenizer
    logger.info(f"Saving model to {model_dir}")
    trainer.save_model(str(model_dir))
    tokenizer.save_pretrained(str(model_dir))
    
    # Save training results
    results = {
        'train_runtime': train_result.training_loss,
        'eval_metrics': eval_result,
        'model_path': str(model_dir),
        'timestamp': datetime.now().isoformat()
    }
    
    results_path = model_dir / "training_results.json"
    with open(results_path, 'w') as f:
        json.dump(results, f, indent=2, default=str)
    
    # Save training configuration
    config = {
        'model_name': MODEL_NAME,
        'max_length': MAX_LENGTH,
        'batch_size': BATCH_SIZE,
        'learning_rate': LEARNING_RATE,
        'num_epochs': NUM_EPOCHS,
        'warmup_steps': WARMUP_STEPS,
        'early_stopping_patience': EARLY_STOPPING_PATIENCE,
        'class_weights': class_weights.cpu().numpy().tolist(),
        'device': device
    }
    save_training_config(model_dir, config)
    
    logger.info("Training completed successfully!")
    logger.info(f"Model saved to: {model_dir}")
    logger.info(f"Final metrics: {eval_result}")
    
    return model_dir

if __name__ == "__main__":
    main()