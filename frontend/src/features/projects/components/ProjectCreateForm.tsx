import { styled } from "styled-components";
import { useState } from "react";
import { FieldError } from "../../auth/components/FieldError";
import { colors, shadows, transitions, fonts, spacing, borderRadius } from "../../../shared/styles/tokens";
import { projectService } from "../services/projectService";

const Form = styled.form`
  background: ${colors.surface};
  max-width: 760px;
  width: 100%;
  border-radius: ${borderRadius.lg};
  padding: ${spacing.xxl};
  font-size: ${fonts.size.base};
  align-self: center;
  box-shadow: ${shadows.sm};
  color: ${colors.textPrimary};
  border: 1px solid ${colors.accentBorderLight};
`;

const Label = styled.label`
  color: ${colors.textPrimary};
  margin: 0 0 ${spacing.md} ${spacing.sm};
  display: block;
  font-size: ${fonts.size.sm};
  font-weight: ${fonts.weight.semibold};
`;

const Input = styled.input`
  background: ${colors.white};
  width: 100%;
  border-radius: ${borderRadius.md};
  height: 44px;
  font-size: ${fonts.size.base};
  border: 1px solid ${colors.accentBorderDark};
  padding: 0 0 0 ${spacing.md};
  outline: none;
  transition: box-shadow ${transitions.fast}, border-color ${transitions.fast};

  &::placeholder { color: ${colors.placeholder} }

  &:focus { box-shadow: 0 8px 30px rgba(37,99,235,0.08); border-color: ${colors.primaryDark} }
`;

const Textarea = styled.textarea`
  background: ${colors.white};
  width: 100%;
  border-radius: ${borderRadius.md};
  min-height: 80px;
  font-size: ${fonts.size.base};
  border: 1px solid ${colors.accentBorderDark};
  padding: ${spacing.md};
  outline: none;
  resize: vertical;
  transition: box-shadow ${transitions.fast}, border-color ${transitions.fast};

  &::placeholder { color: ${colors.placeholder} }

  &:focus { box-shadow: 0 8px 30px rgba(37,99,235,0.08); border-color: ${colors.primaryDark} }
`;

const FileInput = styled.input`
  background: ${colors.white};
  width: 100%;
  border-radius: ${borderRadius.md};
  padding: ${spacing.md};
  font-size: ${fonts.size.base};
  border: 1px solid ${colors.accentBorderDark};
  outline: none;
  transition: box-shadow ${transitions.fast}, border-color ${transitions.fast};

  &:focus { box-shadow: 0 8px 30px rgba(37,99,235,0.08); border-color: ${colors.primaryDark} }
`;

const Button = styled.button`
  width: 100%;
  background: linear-gradient(90deg, ${colors.primary}, ${colors.primaryDark});
  border: none;
  border-radius: ${borderRadius.lg};
  height: 50px;
  color: ${colors.white};
  font-size: ${fonts.size.base};
  margin: ${spacing.md} 0 0 0;
  font-weight: ${fonts.weight.bold};
  box-shadow: 0 10px 24px rgba(37,99,235,0.12);

  &:hover { filter: brightness(1.03); cursor: pointer }
  &:disabled { opacity: 0.6; cursor: not-allowed }
`;

const FieldContainer = styled.div`
  margin: 0 0 ${spacing.lg} 0;
`;

const ImagePreview = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: ${spacing.sm};
  margin-top: ${spacing.md};
`;

const ImagePreviewItem = styled.div`
  position: relative;
  display: inline-block;
`;

const PreviewImage = styled.img`
  width: 80px;
  height: 80px;
  object-fit: cover;
  border-radius: ${borderRadius.md};
  border: 1px solid ${colors.accentBorderLight};
`;

const RemoveButton = styled.button`
  position: absolute;
  top: -5px;
  right: -5px;
  background: ${colors.primary};
  color: ${colors.white};
  border: none;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  cursor: pointer;
  font-size: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
`;

interface ProjectCreateFormProps {
  onSuccess?: () => void;
  onCancel?: () => void;
}

export const ProjectCreateForm = ({ onSuccess, onCancel }: ProjectCreateFormProps) => {
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    externalUrl: '',
  });
  const [files, setFiles] = useState<File[]>([]);
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    // Clear field error when user starts typing
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFiles = e.target.files;
    if (selectedFiles) {
      const newFiles = Array.from(selectedFiles);
      setFiles(prev => [...prev, ...newFiles]);
    }
  };

  const removeImage = (index: number) => {
    setFiles(prev => prev.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setErrors({});

    try {
      // Prepare FormData payload
      const form = new FormData();
      form.append('name', formData.name);
      form.append('description', formData.description);
      if (formData.externalUrl) {
        form.append('externalUrl', formData.externalUrl);
      }
      files.forEach(file => {
        form.append('files', file);
      });

      const result = await projectService.addProject(form);

      if (result.isSuccess) {
        onSuccess?.();
      } else {
        // Handle API errors
        const fieldErrors: Record<string, string> = {};
        result.errors?.forEach(error => {
          const field = error.field?.toLowerCase() || 'general';
          fieldErrors[field] = error.message || 'Unknown error';
        });
        setErrors(fieldErrors);
      }
    } catch (error) {
      console.error('Failed to create project:', error);
      setErrors({ general: 'An unexpected error occurred' });
    } finally {
      setLoading(false);
    }
  };

  return (
    <Form onSubmit={handleSubmit}>
      <FieldContainer>
        <Label>Название проекта *</Label>
        <Input
          type="text"
          name="name"
          placeholder="Введите название проекта"
          value={formData.name}
          onChange={handleInputChange}
          required
        />
        {errors.name && <FieldError message={errors.name} />}
      </FieldContainer>

      <FieldContainer>
        <Label>Описание *</Label>
        <Textarea
          name="description"
          placeholder="Опишите ваш проект"
          value={formData.description}
          onChange={handleInputChange}
          required
        />
        {errors.description && <FieldError message={errors.description} />}
      </FieldContainer>

      <FieldContainer>
        <Label>Ссылка на проект</Label>
        <Input
          type="url"
          name="externalUrl"
          placeholder="https://github.com/your-project"
          value={formData.externalUrl}
          onChange={handleInputChange}
        />
        {errors.externalurl && <FieldError message={errors.externalurl} />}
      </FieldContainer>

      <FieldContainer>
        <Label>Изображения</Label>
        <FileInput
          type="file"
          multiple
          accept="image/*"
          onChange={handleFileChange}
        />
        {files.length > 0 && (
          <ImagePreview>
            {files.map((file, index) => (
              <ImagePreviewItem key={index}>
                <PreviewImage
                  src={URL.createObjectURL(file)}
                  alt={`Preview ${index + 1}`}
                />
                <RemoveButton
                  type="button"
                  onClick={() => removeImage(index)}
                  title="Remove image"
                >
                  ×
                </RemoveButton>
              </ImagePreviewItem>
            ))}
          </ImagePreview>
        )}
        {errors.files && <FieldError message={errors.files} />}
      </FieldContainer>

      {errors.general && <FieldError message={errors.general} />}

      <Button type="submit" disabled={loading}>
        {loading ? 'Создание...' : 'Создать проект'}
      </Button>

      {onCancel && (
        <Button
          type="button"
          onClick={onCancel}
          style={{ background: 'transparent', color: colors.textSecondary, border: `1px solid ${colors.accentBorderDark}` }}
        >
          Отмена
        </Button>
      )}
    </Form>
  );
};
