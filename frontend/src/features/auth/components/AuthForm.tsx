import { styled } from "styled-components";
import type { AuthFormProps } from "../types";
import { FieldError } from "./FieldError";
import { colors, shadows, transitions, fonts, spacing, borderRadius } from "../../../shared/styles/tokens";

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
`;

const FieldContainer = styled.div`
  margin: 0 0 ${spacing.lg} 0;
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

