import React from 'react';
import styled from 'styled-components';
import { colors, transitions, fonts, spacing, borderRadius } from '../../../shared/styles/tokens';

const PaginationContainer = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  gap: ${spacing.sm};
  margin-top: ${spacing.xl};
  margin-bottom: ${spacing.xl};
`;

const PageButton = styled.button<{ isActive?: boolean }>`
  background: ${props => props.isActive ? colors.primary : colors.white};
  color: ${props => props.isActive ? colors.white : colors.textPrimary};
  border: 1px solid ${props => props.isActive ? colors.primary : colors.gray300};
  border-radius: ${borderRadius.md};
  padding: ${spacing.sm} ${spacing.md};
  font-size: ${fonts.size.sm};
  font-weight: ${fonts.weight.medium};
  cursor: pointer;
  transition: all ${transitions.fast};
  min-width: 40px;

  &:hover {
    background: ${props => props.isActive ? colors.primaryDark : colors.gray100};
    border-color: ${props => props.isActive ? colors.primaryDark : colors.gray400};
  }

  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }
`;

const ArrowButton = styled(PageButton)`
  font-weight: ${fonts.weight.bold};
`;

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export const Pagination: React.FC<PaginationProps> = ({
  currentPage,
  totalPages,
  onPageChange
}) => {
  if (totalPages <= 1) return null;

  const getPageNumbers = () => {
    const pages = [];
    const delta = 2; 
    if (1 < currentPage - delta) {
      pages.push(1);
      if (2 < currentPage - delta) {
        pages.push('...');
      }
    }

    for (let i = Math.max(1, currentPage - delta); i <= Math.min(totalPages, currentPage + delta); i++) {
      pages.push(i);
    }

    if (totalPages > currentPage + delta) {
      if (totalPages - 1 > currentPage + delta) {
        pages.push('...');
      }
      pages.push(totalPages);
    }

    return pages;
  };

  const pageNumbers = getPageNumbers();

  return (
    <PaginationContainer>
      <ArrowButton
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage === 1}
      >
        ‹
      </ArrowButton>

      {pageNumbers.map((page, index) => (
        <React.Fragment key={index}>
          {page === '...' ? (
            <span style={{ padding: `0 ${spacing.sm}`, color: colors.gray500 }}>...</span>
          ) : (
            <PageButton
              isActive={page === currentPage}
              onClick={() => onPageChange(page as number)}
            >
              {page}
            </PageButton>
          )}
        </React.Fragment>
      ))}

      <ArrowButton
        onClick={() => onPageChange(currentPage + 1)}
        disabled={currentPage === totalPages}
      >
        ›
      </ArrowButton>
    </PaginationContainer>
  );
};
