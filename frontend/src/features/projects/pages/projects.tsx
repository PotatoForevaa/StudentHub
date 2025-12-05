import { useContext } from "react";
import { ProjectCard } from "../../../components/projects/ProjectCard";
import { CardsContainer, Container } from "../../../components/shared/Container";
import { ProjectContext } from "../../../context/ProjectContext";

export const Projects = () => {
  const { projects } = useContext(ProjectContext);

  return (
    <Container>
      <CardsContainer>
        {projects && projects.length > 0 ? (
          projects.map((p) => <ProjectCard key={p.id} project={p} />)
        ) : (
          <p>No projects found. Check console for API response details.</p>
        )}
      </CardsContainer>
    </Container>
  );
};
