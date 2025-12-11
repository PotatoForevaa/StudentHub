import { styled, keyframes } from 'styled-components';
import { colors } from '../styles/tokens';

const spin = keyframes`
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
`;

const SpinnerContainer = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100px;
  flex-direction: column;
  gap: 12px;
`;

const Spinner = styled.div`
  width: 40px;
  height: 40px;
  border: 3px solid ${colors.gray300};
  border-top: 3px solid ${colors.primary};
  border-radius: 50%;
  animation: ${spin} 1s linear infinite;
`;

const LoadingText = styled.p`
  color: ${colors.textSecondary};
  font-size: 14px;
  margin: 0;
  text-align: center;
`;

interface LoadingSpinnerProps {
  text?: string;
  size?: 'sm' | 'md' | 'lg';
}

export const LoadingSpinner = ({ text = 'Загрузка...', size = 'md' }: LoadingSpinnerProps) => {
  const spinnerSize = {
    sm: '24px',
    md: '40px',
    lg: '56px'
  }[size];

  const borderWidth = size === 'sm' ? '2px' : '3px';

  return (
    <SpinnerContainer>
      <Spinner style={{
        width: spinnerSize,
        height: spinnerSize,
        borderWidth
      }} />
      <LoadingText>{text}</LoadingText>
    </SpinnerContainer>
  );
};
