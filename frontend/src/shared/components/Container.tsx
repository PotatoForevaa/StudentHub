import styled from "styled-components";

const StyledContainer = styled.div`
  background: #190061;
  border-radius: 25px;
  width: 100%;
  margin: 70px 100px;
  padding: 35px;
  display: flex;
  flex-direction: column;
  gap: 20px;
`;

const CardsGrid = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
`;

export const Container = ({ children }: { children?: React.ReactNode }) => {
  return <StyledContainer>{children}</StyledContainer>;
};

export const CardsContainer = ({ children }: { children?: React.ReactNode }) => {
  return <CardsGrid>{children}</CardsGrid>;
};
