import styled from "styled-components";
import { colors, shadows, spacing } from "../../styles/tokens";

const StyledContainer = styled.div`
  background: ${colors.surface};
  border-radius: 12px;
  width: 100%;
  max-width: 1200px;
  margin: ${spacing.xxxl} ${spacing.lg};
  padding: ${spacing.xxl};
  display: flex;
  flex-direction: column;
  gap: ${spacing.lg};
  box-shadow: ${shadows.sm};
  border: 1px solid ${colors.accentBorderLight};
`;

const CardsGrid = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(380px, 1fr));
  gap: ${spacing.lg};
`;

export const Container = ({ children }: { children?: React.ReactNode }) => {
  return <StyledContainer>{children}</StyledContainer>;
};

export const CardsContainer = ({ children }: { children?: React.ReactNode }) => {
  return <CardsGrid>{children}</CardsGrid>;
};
