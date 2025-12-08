import { useContext } from "react";
import { ProjectCard } from "../components/ProjectCard";
import { CardsContainer, Container } from "../../../shared/components/Container";
import { ProjectContext } from "../context/ProjectContext";

export const Projects = () => {
  const { projects, loading } = useContext(ProjectContext);

  return (
    <Container>
      <CardsContainer>
        {loading ? (
          <p>Loading projects...</p>
        ) : projects && projects.length > 0 ? (
          projects.map((p) => <ProjectCard key={p.id} project={p} />)
        ) : (
          <p>No projects found.</p>
        )}
      </CardsContainer>
    </Container>
  );
};
