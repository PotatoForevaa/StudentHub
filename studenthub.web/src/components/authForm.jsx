import { styled } from "styled-components";

export const AuthForm = () => {
  const Form = styled.div`
    background: #F8F9FA;
    width: 500px;
    height: 599px;
    border-radius: 25px;
    padding: 10px 15px 15px 15px;    
    font-size: 28px;
  `;

  const Label = styled.label`
    color: #212529;
  `;

  const Input = styled.input`
    background: #FFFFFF;
    width: 100%;
    border-radius: 10px;
    height: 30px;
    font-size: 24px;
    margin-bottom: 10px;
    border: 1px solid;
  `;

  const Button = styled.button`
    width: 100%;
    background: #2C5AA0;
    border: none;
    border-radius: 15px;
    height: 30px;
    color: #F8F8F8;
    font-size: 20px;
  `;

  return (
      <Form>
        <Label>gegeg</Label>
        <Input></Input>
        <Label>gegeg</Label>
        <Input></Input>
        <Label>gegeg</Label>
        <Input></Input>
        <Button>Вход</Button>
      </Form>
  );
};
