export interface AuthFormProps{
    buttonText: string,
    onSubmit: (event: React.FormEvent<HTMLFormElement>) => void,
    fields: Field[],
    fieldErrors?: FieldErrors;
    formError?: string | null;
};

export interface Field{
    displayName: string,
    name: string,
    type: string,
    placeholder: string,
    onChange: (event: React.ChangeEvent<HTMLInputElement>) => void
};

export type FieldErrors = Record<string, string>;