import { useState, useEffect, useCallback } from 'react';

interface UsePaginationReturn<T> {
  paginatedItems: T[];
  currentPage: number;
  pageSize: number;
  totalPages: number;
  setCurrentPage: (page: number) => void;
  setPageSize: (size: number) => void;
}

/**
 * Custom hook for client-side pagination
 * @param items - Array of items to paginate
 * @param initialPageSize - Initial page size (default: 6)
 * @returns Pagination state and computed values
 */
export function usePagination<T>(
  items: T[],
  initialPageSize: number = 6
): UsePaginationReturn<T> {
  const [currentPage, setCurrentPageState] = useState(1);
  const [pageSize, setPageSizeState] = useState(initialPageSize);

  const totalPages = Math.ceil(items.length / pageSize);
  const paginatedItems = items.slice((currentPage - 1) * pageSize, currentPage * pageSize);

  const setCurrentPage = useCallback((page: number) => {
    setCurrentPageState(page);
  }, []);

  const setPageSize = useCallback((size: number) => {
    setPageSizeState(size);
    setCurrentPageState(1);
  }, []);

  // Reset to page 1 if current page exceeds total pages (e.g., after filtering)
  useEffect(() => {
    if (currentPage > totalPages && totalPages > 0) {
      setCurrentPageState(1);
    }
  }, [currentPage, totalPages]);

  return {
    paginatedItems,
    currentPage,
    pageSize,
    totalPages,
    setCurrentPage,
    setPageSize,
  };
}
