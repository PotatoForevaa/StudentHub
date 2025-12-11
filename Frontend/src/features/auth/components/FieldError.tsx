import { styled } from "styled-components";
import { colors } from "../../../shared/styles/tokens";

const Error = styled.p`
  color: ${colors.primary};
  margin: 0 0 0 10px;
  font-size: 12px;
`;

export const FieldError = ({ message }: { message: string }) => {
  if (!message) return null;

  return <Error>{message}</Error>;
};

