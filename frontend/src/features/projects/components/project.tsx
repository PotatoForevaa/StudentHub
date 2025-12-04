import styled from "styled-components";
import type { Project } from "../../../shared/types/Project";
import { baseUrl } from "../../../services/api/base";

const Card = styled.div`
  background: #3500d3;
  color: #fff;
  width: 300px;
  border-radius: 10px;
  padding: 16px;
  box-shadow: 0 4px 10px rgba(0,0,0,0.2);
  display: flex;
  flex-direction: column;
  gap: 10px;
`;

const Title = styled.h3`
  margin: 0;
`;

const Description = styled.p`
  margin: 0;
  font-size: 14px;
  color: #e0e0ff;
`;

const AuthorDate = styled.div`
  font-size: 12px;
  color: #bbb;
  display: flex;
  justify-content: space-between;
`;

const ImagesContainer = styled.div`
  display: flex;
  gap: 5px;
`;

const Image = styled.img`
  width: 50px;
  height: 50px;
  border-radius: 5px;
  object-fit: cover;
`;

interface ProjectProps {
  project: Project;
}

export const ProjectCard = ({ project }: ProjectProps) => {
  return (
    <Card>
      <Title>{project.name}</Title>
      <Description>{project.description.slice(0, 100)}...</Description>
      <ImagesContainer>
        {project.files?.map((img, idx) => (
          <Image key={idx} src={`${baseUrl}/api/projects/${project.id}/${img}`} alt={`Project image ${idx + 1}`} />
        ))}
      </ImagesContainer>
      <AuthorDate>
        <span>{project.author}</span>
        <span>дата</span>
      </AuthorDate>
    </Card>
  );
};
