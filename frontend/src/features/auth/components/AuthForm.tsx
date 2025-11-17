import { styled } from "styled-components";
import type { AuthFormProps } from "../types/AuthForm.types";

const Form = styled.form`
  background: #190061;
  max-width: 500px;
  border-radius: 25px;
  padding: 10px 15px 15px 15px;
  font-size: 24px;
`;

const Label = styled.label`
  color: #FFFFFF;
  margin: 0 0 0 5px;
`;

const Input = styled.input`
  background: #ffffff;
  width: 100%;
  border-radius: 10px;
  height: 30px;
  font-size: 22px;
  margin-bottom: 10px;
  border: 1px solid;
  padding: 0 0 0 5px;
`;

const Button = styled.button`
  width: 100%;
  background: #3500D3;
  border: none;
  border-radius: 15px;
  height: 45px;
  color: #f8f8f8;
  font-size: 20px;
  margin: 10px 0 0 0;

  &:hover {
    background: #4A1AFF;
    cursor: pointer;
  }
`;

const FieldError = styled.div`
  width: 100%;
  background: red;
  border-radius: 15px;
  height: 45px;
`;

export const AuthForm = (props: AuthFormProps) => {
  const { fieldErrors, formError } = props;

  return (
    <Form onSubmit={props.onSubmit}>
      {props.fields.map((field) => (
        <div key={field.name}>
          <Label>{field.name}</Label>
          <Input
            type={field.type}
            placeholder={field.placeholder}
            onChange={field.onChange}
          />
          {fieldErrors?.[field.name] && (
            <FieldError>{fieldErrors[field.name]}</FieldError>
          )}
        </div>
      ))}

      {formError && <FieldError>{formError}</FieldError>}

      <Button type="submit">{props.buttonText}</Button>
    </Form>
  );
};

