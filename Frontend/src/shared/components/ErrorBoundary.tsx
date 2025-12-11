import { Component, ReactNode } from 'react';
import { styled } from 'styled-components';
import { colors, fonts } from '../styles/tokens';

const ErrorContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 300px;
  padding: 2rem;
  background: ${colors.white};
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
`;

const ErrorTitle = styled.h2`
  color: #ef4444;
  font-size: ${fonts.size.xl};
  font-weight: ${fonts.weight.semibold};
  margin-bottom: 1rem;
`;

const ErrorMessage = styled.p`
  color: ${colors.textPrimary};
  margin-bottom: 1.5rem;
  text-align: center;
`;

const RetryButton = styled.button`
  background: ${colors.primary};
  color: ${colors.white};
  border: none;
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: ${fonts.weight.medium};
  cursor: pointer;
  transition: background-color 0.2s;

  &:hover {
    background: ${colors.primaryDark};
  }
`;

interface ErrorBoundaryState {
  hasError: boolean;
  error: Error | null;
}

interface ErrorBoundaryProps {
  children: ReactNode;
  fallback?: ReactNode;
}

export class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  constructor(props: ErrorBoundaryProps) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error: Error): ErrorBoundaryState {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, errorInfo: any) {
    console.error('Error Boundary caught an error:', error, errorInfo);
  }

  handleRetry = () => {
    this.setState({ hasError: false, error: null });
  };

  render() {
    if (this.state.hasError) {
      if (this.props.fallback) {
        return this.props.fallback;
      }

      return (
        <ErrorContainer>
          <ErrorTitle>Что-то пошло не так</ErrorTitle>
          <ErrorMessage>
            Произошла ошибка при загрузке компонента. Попробуйте обновить страницу.
          </ErrorMessage>
          <RetryButton onClick={this.handleRetry}>
            Попробовать снова
          </RetryButton>
          {process.env.NODE_ENV === 'development' && this.state.error && (
            <details style={{ marginTop: '1rem', width: '100%' }}>
              <summary>Подробности ошибки (только в разработке)</summary>
              <pre style={{
                background: colors.surface,
                padding: '1rem',
                borderRadius: '4px',
                fontSize: '0.875rem',
                overflow: 'auto',
                marginTop: '0.5rem'
              }}>
                {this.state.error.message}
              </pre>
            </details>
          )}
        </ErrorContainer>
      );
    }

    return this.props.children;
  }
}
