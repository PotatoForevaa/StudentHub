import { useState, useEffect, useContext } from "react";
import { ProjectCard } from "../components/ProjectCard";
import { ProjectCreateForm } from "../components/ProjectCreateForm";
import { Pagination } from "../components/Pagination";
import { CardsContainer, Container } from "../../../shared/components/Container";
import { useProjects } from "../hooks/useProjects";
import { AuthContext } from "../../auth/context/AuthContext";
import { LoadingSpinner } from "../../../shared/components/LoadingSpinner";
import { styled } from "styled-components";
import { colors, shadows, transitions, fonts, spacing, borderRadius } from "../../../shared/styles/tokens";

const Header = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: ${spacing.lg};
`;

const Title = styled.h1`
  color: ${colors.textPrimary};
  font-size: ${fonts.size['2xl']};
  font-weight: ${fonts.weight.bold};
  margin: 0;
`;

const CreateButton = styled.button`
  background: linear-gradient(90deg, ${colors.primary}, ${colors.primaryDark});
  border: none;
  border-radius: ${borderRadius.md};
  padding: ${spacing.md} ${spacing.xl};
  color: ${colors.white};
  font-weight: ${fonts.weight.semibold};
  font-size: ${fonts.size.base};
  cursor: pointer;
  box-shadow: ${shadows.sm};
  transition: all ${transitions.base};

  &:hover {
    filter: brightness(1.05);
    transform: translateY(-1px);
    box-shadow: 0 6px 20px rgba(37,99,235,0.15);
  }
`;

const FiltersContainer = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: ${spacing.md};
  margin-bottom: ${spacing.lg};
  align-items: flex-end;
`;

const FilterGroup = styled.div`
  display: flex;
  flex-direction: column;
  gap: ${spacing.xs};
  flex: 1;
  min-width: 200px;
`;

const FilterLabel = styled.label`
  font-size: ${fonts.size.sm};
  color: ${colors.textSecondary};
  font-weight: ${fonts.weight.medium};
`;

const SearchInput = styled.input`
  background: ${colors.surface};
  color: ${colors.textPrimary};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.md};
  padding: ${spacing.sm} ${spacing.md};
  font-size: ${fonts.size.base};
  outline: none;
  transition: border-color ${transitions.fast};

  &::placeholder { color: ${colors.muted}; }
  &:focus { border-color: ${colors.primary}; }
`;

const Select = styled.select`
  background: ${colors.surface};
  color: ${colors.textPrimary};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.md};
  padding: ${spacing.sm} ${spacing.md};
  font-size: ${fonts.size.base};
  outline: none;
  cursor: pointer;
  transition: border-color ${transitions.fast};

  &:focus { border-color: ${colors.primary}; }
`;

const FilterButton = styled.button`
  background: ${colors.primary};
  color: ${colors.white};
  border: none;
  border-radius: ${borderRadius.md};
  padding: ${spacing.sm} ${spacing.lg};
  font-size: ${fonts.size.base};
  font-weight: ${fonts.weight.semibold};
  cursor: pointer;
  transition: background ${transitions.base};
  height: 40px;

  &:hover {
    background: ${colors.primaryDark};
  }
`;

const ResetButton = styled.button`
  background: transparent;
  color: ${colors.textSecondary};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.md};
  padding: ${spacing.sm} ${spacing.lg};
  font-size: ${fonts.size.base};
  font-weight: ${fonts.weight.semibold};
  cursor: pointer;
  transition: all ${transitions.base};
  height: 40px;

  &:hover {
    border-color: ${colors.primary};
    color: ${colors.primary};
  }
`;

const InfoText = styled.p`
  color: ${colors.textSecondary};
  text-align: center;
  padding: ${spacing.xl};
  font-size: ${fonts.size.base};
`;

const Projects = () => {
  const { isAuthenticated, loading: authLoading } = useContext(AuthContext);
  const [showCreateForm, setShowCreateForm] = useState(false);

  const {
    projects,
    paginatedProjects,
    loading: projectsLoading,
    refetch: getProjects,
    currentPage,
    totalPages,
    setCurrentPage,
    setSearchTerm,
    selectedCategoryId,
    setSelectedCategoryId,
    selectedTagId,
    setSelectedTagId,
    categories,
    tags,
  } = useProjects();

  const [localSearchTerm, setLocalSearchTerm] = useState("");

  useEffect(() => {
    if (!authLoading && isAuthenticated && projects?.length === 0) {
      getProjects();
    }
  }, [isAuthenticated, authLoading, getProjects]);

  useEffect(() => {
    if (projects && projects.length > 0 && currentPage > totalPages && totalPages > 0) {
      setCurrentPage(1);
    }
  }, [projects, currentPage, totalPages, setCurrentPage]);

  const handleSearch = () => {
    setSearchTerm(localSearchTerm);
    setCurrentPage(1);
    getProjects();
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleSearch();
    }
  };

  const handleReset = () => {
    setLocalSearchTerm("");
    setSearchTerm("");
    setSelectedCategoryId(undefined);
    setSelectedTagId(undefined);
    setCurrentPage(1);
    setTimeout(() => getProjects(), 0);
  };

  const handleCreateSuccess = () => {
    setShowCreateForm(false);
    getProjects();
  };

  const handleCancel = () => {
    setShowCreateForm(false);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  return (
    <Container>
      <Header>
        <Title>Проекты</Title>
        <CreateButton onClick={() => setShowCreateForm(true)}>
          Создать проект
        </CreateButton>
      </Header>

      {showCreateForm && (
        <ProjectCreateForm
          onSuccess={handleCreateSuccess}
          onCancel={handleCancel}
        />
      )}

      <FiltersContainer>
        <FilterGroup>
          <FilterLabel>Поиск по названию</FilterLabel>
          <SearchInput
            type="text"
            placeholder="🔍 Введите название проекта..."
            value={localSearchTerm}
            onChange={(e) => setLocalSearchTerm(e.target.value)}
            onKeyDown={handleKeyDown}
          />
        </FilterGroup>

        <FilterGroup>
          <FilterLabel>Категория</FilterLabel>
          <Select
            value={selectedCategoryId || ""}
            onChange={(e) => setSelectedCategoryId(e.target.value || undefined)}
          >
            <option value="">Все категории</option>
            {categories.map((cat) => (
              <option key={cat.id} value={cat.id}>{cat.name}</option>
            ))}
          </Select>
        </FilterGroup>

        <FilterGroup>
          <FilterLabel>Тег</FilterLabel>
          <Select
            value={selectedTagId || ""}
            onChange={(e) => setSelectedTagId(e.target.value || undefined)}
          >
            <option value="">Все теги</option>
            {tags.map((tag) => (
              <option key={tag.id} value={tag.id}>{tag.name}</option>
            ))}
          </Select>
        </FilterGroup>

        <FilterButton onClick={handleSearch}>Найти</FilterButton>
        <ResetButton onClick={handleReset}>Сбросить</ResetButton>
      </FiltersContainer>

      <CardsContainer>
        {projectsLoading ? (
          <LoadingSpinner text="Загрузка проектов..." size="md" />
        ) : paginatedProjects && paginatedProjects.length > 0 ? (
          paginatedProjects.map((p) => <ProjectCard key={p.id} project={p} />)
        ) : projects && projects.length > 0 ? (
          <InfoText>На этой странице нет проектов.</InfoText>
        ) : (
          <InfoText>Проекты не найдены. Попробуйте изменить параметры поиска.</InfoText>
        )}
      </CardsContainer>

      {!projectsLoading && projects && projects.length > 0 && (
        <Pagination
          currentPage={currentPage}
          totalPages={totalPages}
          onPageChange={handlePageChange}
        />
      )}
    </Container>
  );
};

export default Projects;