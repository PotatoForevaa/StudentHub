import { useContext } from "react";
import { ProjectCard } from "../components/project";
import { CardsContainer, Container } from "../../../shared/components/Container";
import { ProjectContext } from "../../../shared/context/ProjectContext";

export const Projects = () => {
  const { projects } = useContext(ProjectContext);

  return (
    <Container>
      <CardsContainer>
        {projects?.map((p) => (
          <ProjectCard key={p.id} project={p} />
        ))}
      </CardsContainer>
    </Container>
  );
};
