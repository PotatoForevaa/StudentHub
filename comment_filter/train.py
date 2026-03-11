import pandas as pd
import torch
from torch.utils.data import Dataset
from torch import nn
from transformers import AutoTokenizer, AutoModelForSequenceClassification, Trainer, TrainingArguments
from sklearn.model_selection import train_test_split
from sklearn.utils.class_weight import compute_class_weight
from sklearn.metrics import accuracy_score, f1_score
import numpy as np

device = "cuda" if torch.cuda.is_available() else "cpu"
MODEL_NAME = "DeepPavlov/rubert-base-cased"

data = pd.read_csv("toxic_comments.csv")

classes = np.array([0, 1])
weights = compute_class_weight("balanced", classes=classes, y=data['toxic'].values)
class_weights = torch.tensor(weights, dtype=torch.float).to(device)
print(f"Class weights: {class_weights}")

train_texts, test_texts, train_labels, test_labels = train_test_split(
    data['comment'].astype(str).tolist(),
    data['toxic'].astype(int).tolist(),
    test_size=0.2,
    random_state=42
)

tokenizer = AutoTokenizer.from_pretrained(MODEL_NAME)

class ToxicDataset(Dataset):
    def __init__(self, texts, labels):
        self.encodings = tokenizer(texts, truncation=True, padding=True, max_length=128)
        self.labels = labels

    def __getitem__(self, idx):
        item = {key: torch.tensor(val[idx]) for key, val in self.encodings.items()}
        item["labels"] = torch.tensor(self.labels[idx])
        return item

    def __len__(self):
        return len(self.labels)

train_dataset = ToxicDataset(train_texts, train_labels)
test_dataset = ToxicDataset(test_texts, test_labels)

model = AutoModelForSequenceClassification.from_pretrained(
    MODEL_NAME,
    num_labels=2
)
model.to(device)

def compute_metrics(pred):
    logits, labels = pred
    preds = logits.argmax(-1)
    acc = accuracy_score(labels, preds)
    f1 = f1_score(labels, preds, average="binary")
    return {"accuracy": acc, "f1": f1}

class WeightedTrainer(Trainer):
    def compute_loss(self, model, inputs, return_outputs=False, **kwargs):
        labels = inputs.get("labels")
        outputs = model(**inputs)
        logits = outputs.logits
        loss_fn = nn.CrossEntropyLoss(weight=class_weights)
        loss = loss_fn(logits, labels)
        return (loss, outputs) if return_outputs else loss

training_args = TrainingArguments(
    output_dir="./results",
    num_train_epochs=3,
    per_device_train_batch_size=16, 
    per_device_eval_batch_size=16,
    save_strategy="epoch",
    logging_strategy="steps",
    logging_steps=100,
    save_total_limit=2,
    fp16=True,
    eval_strategy="epoch",
    load_best_model_at_end=True
)

trainer = WeightedTrainer(
    model=model,
    args=training_args,
    train_dataset=train_dataset,
    eval_dataset=test_dataset,
    compute_metrics=compute_metrics
)

trainer.train()

model.save_pretrained("toxic_model_balanced")
tokenizer.save_pretrained("toxic_model_balanced")
print("Training finished")