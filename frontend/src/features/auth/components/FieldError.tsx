import { styled } from "styled-components";

const Error = styled.p`
  color: red;
  margin: 0 0 0 10px;
`;

export const FieldError = ({ message }: { message: string }) => {
  if (!message) return null;

  return <Error>{message}</Error>;
};
