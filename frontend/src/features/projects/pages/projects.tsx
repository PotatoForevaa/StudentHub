import { useContext, useState, useEffect } from "react";
import { ProjectCard } from "../components/ProjectCard";
import { ProjectCreateForm } from "../components/ProjectCreateForm";
import { Pagination } from "../components/Pagination";
import { CardsContainer, Container } from "../../../shared/components/Container";
import { ProjectContext } from "../context/ProjectContext";
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

export const Projects = () => {
  const {
    projects,
    paginatedProjects,
    loading,
    getProjects,
    currentPage,
    totalPages,
    setCurrentPage
  } = useContext(ProjectContext);
  const [showCreateForm, setShowCreateForm] = useState(false);

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

  useEffect(() => {
    if (projects && projects.length > 0 && currentPage > totalPages && totalPages > 0) {
      setCurrentPage(1);
    }
  }, [projects, currentPage, totalPages, setCurrentPage]);

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

      <CardsContainer>
        {loading ? (
          <p>Loading projects...</p>
        ) : paginatedProjects && paginatedProjects.length > 0 ? (
          paginatedProjects.map((p) => <ProjectCard key={p.id} project={p} />)
        ) : projects && projects.length > 0 ? (
          <p>No projects found on this page.</p>
        ) : (
          <p>No projects found.</p>
        )}
      </CardsContainer>

      {!loading && projects && projects.length > 0 && (
        <Pagination
          currentPage={currentPage}
          totalPages={totalPages}
          onPageChange={handlePageChange}
        />
      )}
    </Container>
  );
};
