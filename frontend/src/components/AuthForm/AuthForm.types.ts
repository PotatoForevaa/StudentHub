export interface AuthFormProps{
    buttonText: string,
    onSubmit: (event: React.FormEvent<HTMLFormElement>) => void,
    fields: Field[]
};

export interface Field{
    name: string,
    type: string,
    placeholder: string,
    onChange: (event: React.ChangeEvent<HTMLInputElement>) => void
};

