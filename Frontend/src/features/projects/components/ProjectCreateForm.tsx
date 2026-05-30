import { styled } from "styled-components";
import { useState, useEffect } from "react";
import { FieldError } from "../../auth/components/FieldError";
import { colors, shadows, fonts, spacing, borderRadius, transitions } from "../../../shared/styles/tokens";
import { useProjectForm } from "../hooks/useProjectForm";
import { projectService } from "../services/projectService";
import type { ProjectFormData, CategoryDto, TagDto } from "../types";

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

const Select = styled.select`
  background: ${colors.white};
  width: 100%;
  border-radius: ${borderRadius.md};
  height: 44px;
  font-size: ${fonts.size.base};
  border: 1px solid ${colors.accentBorderDark};
  padding: 0 0 0 ${spacing.md};
  outline: none;
  cursor: pointer;
  transition: box-shadow ${transitions.fast}, border-color ${transitions.fast};

  &:focus { box-shadow: 0 8px 30px rgba(37,99,235,0.08); border-color: ${colors.primaryDark} }
`;

const MultiSelectContainer = styled.div`
  background: ${colors.white};
  width: 100%;
  border-radius: ${borderRadius.md};
  min-height: 44px;
  font-size: ${fonts.size.base};
  border: 1px solid ${colors.accentBorderDark};
  padding: ${spacing.sm};
  outline: none;
  display: flex;
  flex-wrap: wrap;
  gap: ${spacing.xs};
  cursor: pointer;
  transition: box-shadow ${transitions.fast}, border-color ${transitions.fast};

  &:focus-within { box-shadow: 0 8px 30px rgba(37,99,235,0.08); border-color: ${colors.primaryDark} }
`;

const SelectedBadge = styled.span`
  background: ${colors.primaryLight || '#dbeafe'};
  color: ${colors.primaryDark || '#1e40af'};
  font-size: ${fonts.size.sm};
  padding: 2px ${spacing.sm};
  border-radius: ${borderRadius.sm};
  display: inline-flex;
  align-items: center;
  gap: ${spacing.xs};
`;

const RemoveBadgeButton = styled.button`
  background: none;
  border: none;
  cursor: pointer;
  color: ${colors.primaryDark || '#1e40af'};
  font-size: 14px;
  padding: 0;
  line-height: 1;
  display: flex;
  align-items: center;
`;

const Dropdown = styled.div`
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: ${colors.white};
  border: 1px solid ${colors.accentBorderDark};
  border-radius: ${borderRadius.md};
  max-height: 200px;
  overflow-y: auto;
  z-index: 10;
  box-shadow: ${shadows.md};
`;

const DropdownItem = styled.div<{ selected: boolean }>`
  padding: ${spacing.sm} ${spacing.md};
  cursor: pointer;
  background: ${props => props.selected ? (colors.primaryLight || '#dbeafe') : 'transparent'};
  color: ${colors.textPrimary};
  transition: background ${transitions.fast};

  &:hover {
    background: ${props => props.selected ? (colors.primaryLight || '#dbeafe') : (colors.gray100 || '#f3f4f6')};
  }
`;

const DropdownWrapper = styled.div`
  position: relative;
`;

const Checkbox = styled.input`
  margin-right: ${spacing.sm};
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

interface MultiSelectProps {
  label: string;
  options: { id: string; name: string }[];
  selectedIds: string[];
  onChange: (ids: string[]) => void;
  error?: string;
}

function MultiSelect({ label, options, selectedIds, onChange, error }: MultiSelectProps) {
  const [isOpen, setIsOpen] = useState(false);

  const toggleOption = (id: string) => {
    if (selectedIds.includes(id)) {
      onChange(selectedIds.filter(i => i !== id));
    } else {
      onChange([...selectedIds, id]);
    }
  };

  const selectedNames = options
    .filter(o => selectedIds.includes(o.id))
    .map(o => o.name);

  return (
    <DropdownWrapper>
      <Label>{label}</Label>
      <MultiSelectContainer onClick={() => setIsOpen(!isOpen)}>
        {selectedNames.length === 0 ? (
          <span style={{ color: colors.placeholder, fontSize: fonts.size.base }}>
            Выберите...
          </span>
        ) : (
          selectedNames.map((name, idx) => (
            <SelectedBadge key={idx}>
              {name}
              <RemoveBadgeButton
                type="button"
                onClick={(e) => {
                  e.stopPropagation();
                  const item = options.find(o => o.name === name);
                  if (item) onChange(selectedIds.filter(i => i !== item.id));
                }}
              >
                ×
              </RemoveBadgeButton>
            </SelectedBadge>
          ))
        )}
      </MultiSelectContainer>
      {isOpen && (
        <Dropdown>
          {options.map((option) => (
            <DropdownItem
              key={option.id}
              selected={selectedIds.includes(option.id)}
              onClick={() => toggleOption(option.id)}
            >
              <Checkbox
                type="checkbox"
                checked={selectedIds.includes(option.id)}
                readOnly
              />
              {option.name}
            </DropdownItem>
          ))}
          {options.length === 0 && (
            <DropdownItem selected={false}>Нет доступных опций</DropdownItem>
          )}
        </Dropdown>
      )}
      {isOpen && (
        <div
          style={{ position: 'fixed', top: 0, left: 0, right: 0, bottom: 0, zIndex: 9 }}
          onClick={() => setIsOpen(false)}
        />
      )}
      {error && <FieldError message={error} />}
    </DropdownWrapper>
  );
}

export const ProjectCreateForm = ({ onSuccess, onCancel }: ProjectCreateFormProps) => {
  const { onSubmit, loading, fieldErrors, formError } = useProjectForm(onSuccess);
  const [formData, setFormData] = useState<ProjectFormData>({
    name: '',
    description: '',
    externalUrl: '',
    categoryId: '',
    tagIds: [],
  });
  const [files, setFiles] = useState<File[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [tags, setTags] = useState<TagDto[]>([]);

  useEffect(() => {
    const loadFilters = async () => {
      try {
        const [catResult, tagResult] = await Promise.all([
          projectService.getCategories(),
          projectService.getTags(),
        ]);
        if (catResult.isSuccess && catResult.data) {
          setCategories(catResult.data);
        }
        if (tagResult.isSuccess && tagResult.data) {
          setTags(tagResult.data);
        }
      } catch (err) {
        console.error("Failed to load categories/tags", err);
      }
    };
    loadFilters();
  }, []);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
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

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData, files);
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
        {fieldErrors.name && <FieldError message={fieldErrors.name} />}
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
        {fieldErrors.description && <FieldError message={fieldErrors.description} />}
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
        {fieldErrors.externalurl && <FieldError message={fieldErrors.externalurl} />}
      </FieldContainer>

      <FieldContainer>
        <Label>Категория *</Label>
        <Select
          value={formData.categoryId || ""}
          onChange={(e) => setFormData(prev => ({ ...prev, categoryId: e.target.value }))}
          required
        >
          <option value="">-- Выберите категорию --</option>
          {categories.map((cat) => (
            <option key={cat.id} value={cat.id}>{cat.name}</option>
          ))}
        </Select>
        {fieldErrors.categoryid && <FieldError message={fieldErrors.categoryid} />}
      </FieldContainer>

      <FieldContainer>
        <MultiSelect
          label="Теги"
          options={tags}
          selectedIds={formData.tagIds || []}
          onChange={(ids) => setFormData(prev => ({ ...prev, tagIds: ids }))}
          error={fieldErrors.tagids}
        />
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
                  alt={`Превью ${index + 1}`}
                />
                <RemoveButton
                  type="button"
                  onClick={() => removeImage(index)}
                  title="Удалить изображение"
                >
                  ×
                </RemoveButton>
              </ImagePreviewItem>
            ))}
          </ImagePreview>
        )}
        {fieldErrors.files && <FieldError message={fieldErrors.files} />}
      </FieldContainer>

      {formError && <FieldError message={formError} />}

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