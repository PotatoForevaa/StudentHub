export const validationRules = {
  required: (value: string) => {
    return value.trim() ? null : 'Это поле обязательно';
  },

  minLength: (minLength: number) => (value: string) => {
    return value.length >= minLength ? null : `Минимум ${minLength} символов`;
  },

  maxLength: (maxLength: number) => (value: string) => {
    return value.length <= maxLength ? null : `Максимум ${maxLength} символов`;
  },

  email: (value: string) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(value) ? null : 'Введите корректный email';
  },

  username: (value: string) => {
    const usernameRegex = /^[a-zA-Z0-9_]{3,20}$/;
    return usernameRegex.test(value) ? null : 'Имя пользователя должно содержать 3-20 символов (буквы, цифры, _)';
  },

  password: (value: string) => {
    const hasLowerCase = /[a-z]/.test(value);
    const hasUpperCase = /[A-Z]/.test(value);
    const hasNumber = /\d/.test(value);
    const hasMinLength = value.length >= 8;

    if (!(hasLowerCase && hasUpperCase && hasNumber && hasMinLength)) {
      return 'Пароль должен содержать минимум 8 символов, включая заглавную букву, строчную букву и цифру';
    }
    return null;
  },

  url: (value: string) => {
    if (!value.trim()) return null; // Allow empty optional URLs
    try {
      new URL(value);
      return null;
    } catch {
      return 'Введите корректный URL';
    }
  }
};

export const validateField = (value: string, rules: ((value: string) => string | null)[]): string | null => {
  for (const rule of rules) {
    const error = rule(value);
    if (error) return error;
  }
  return null;
};

export const validateForm = (
  formData: Record<string, string>,
  schema: Record<string, ((value: string) => string | null)[]>
): Record<string, string> => {
  const errors: Record<string, string> = {};

  for (const [fieldName, rules] of Object.entries(schema)) {
    const error = validateField(formData[fieldName] || '', rules);
    if (error) {
      errors[fieldName] = error;
    }
  }

  return errors;
};
