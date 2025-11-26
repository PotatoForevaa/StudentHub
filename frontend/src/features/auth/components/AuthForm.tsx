import { styled } from "styled-components";
import type { AuthFormProps } from "../types/AuthForm.types";
import { FieldError } from "./FieldError";

const Form = styled.form`
  background: #190061;
  max-width: 500px;
  border-radius: 25px;
  padding: 10px 15px 15px 15px;
  font-size: 24px;
  align-self: center;
`;

const Label = styled.label`
  color: #FFFFFF;
  margin: 0 0 0 10px;
`;

const Input = styled.input`
  background: #ffffff;
  width: 100%;
  border-radius: 10px;
  height: 30px;
  font-size: 22px;
  border: 1px solid;
  padding: 0 0 0 10px;
`;

const Button = styled.button`
  width: 100%;
  background: #3500D3;
  border: none;
  border-radius: 15px;
  height: 45px;
  color: #f8f8f8;
  font-size: 20px;
  margin: 0 0 0 0;

  &:hover {
    background: #4A1AFF;
    cursor: pointer;
  }
`;

const FieldContainer = styled.div`
  margin: 0 0 10px 0;
`;


export const AuthForm = (props: AuthFormProps) => {
  const { fieldErrors, formError } = props;

  return (
    <Form onSubmit={props.onSubmit}>
      {props.fields.map((field) => (
        <FieldContainer key={field.name}>
          <Label>{field.displayName}</Label>
          <Input
            type={field.type}
            placeholder={field.placeholder}
            onChange={field.onChange}
          />
          {fieldErrors?.[field.name.toLowerCase()] && (
            <FieldError message={fieldErrors[field.name.toLowerCase()]} />
          )}
        </FieldContainer>
      ))}

      {formError && <FieldError message={formError} />}

      <Button type="submit">{props.buttonText}</Button>
    </Form>
  );
};

