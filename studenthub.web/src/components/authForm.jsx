import { styled } from "styled-components";
const Form = styled.div`
  background: #f8f9fa;
  max-width: 500px;
  border-radius: 25px;
  padding: 10px 15px 15px 15px;
  font-size: 24px;
`;

const Label = styled.label`
  color: #212529;
  margin: 0 0 0 5px;
`;

const Input = styled.input`
  background: #ffffff;
  width: 100%;
  border-radius: 10px;
  height: 30px;
  font-size: 24px;
  margin-bottom: 10px;
  border: 1px solid;
  padding: 0 0 0 5px;
`;

const Button = styled.button`
  width: 100%;
  background: #2c5aa0;
  border: none;
  border-radius: 15px;
  height: 45px;
  color: #f8f8f8;
  font-size: 20px;
  margin: 10px 0 0 0;

  &:hover {
    background: black;
  }
`;

export const AuthForm = ({ buttonText, onSubmit, fields = [] }) => {
  return (
    <Form>
      {fields.map((field) => (
        <div key={field.name}>
          <Label>{field.name}</Label>
          <Input
            type={field.type}
            placeholder={field.placeholder}
            onChange={field.onChange}
          />
        </div>
      ))}
      <Button onClick={onSubmit}>{buttonText}</Button>
    </Form>
  );
};
