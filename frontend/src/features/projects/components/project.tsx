import styled from "styled-components";
import type { Project } from "../../../shared/types/Project";
import { baseUrl } from "../../../services/api/base";
import { colors, shadows, fonts, spacing, borderRadius, transitions } from "../../../styles/tokens";

const Card = styled.div`
  background: ${colors.surface};
  color: ${colors.textPrimary};
  width: 100%;
  min-height: 240px;
  border-radius: ${borderRadius.lg};
  padding: ${spacing.lg};
  box-shadow: ${shadows.sm};
  display: flex;
  flex-direction: column;
  gap: ${spacing.md};
  transition: transform ${transitions.base}, box-shadow ${transitions.base};

  border-left: 4px solid ${colors.accentBorder};
  &:hover { transform: translateY(-6px); box-shadow: ${shadows.lg} }
`;

const Title = styled.h3`
  margin: 0;
  font-size: ${fonts.size.xl};
  letter-spacing: 0.2px;
  color: ${colors.textPrimary};
  font-weight: ${fonts.weight.semibold};
`;

const Description = styled.p`
  margin: 0;
  font-size: ${fonts.size.base};
  color: ${colors.textSecondary};
  line-height: 1.45;
`;

const AuthorDate = styled.div`
  font-size: ${fonts.size.xs};
  color: ${colors.muted};
  display: flex;
  justify-content: space-between;
  margin-top: auto;
`;

const ImagesContainer = styled.div`
  display: flex;
  gap: 12px;
  align-items: center;
`;

const Image = styled.img`
  width: 160px;
  height: 110px;
  border-radius: ${borderRadius.sm};
  object-fit: cover;
  border: 1px solid ${colors.accentBorderDark};
  box-shadow: 0 6px 18px rgba(2,6,23,0.04);
`;

interface ProjectProps {
  project: Project;
}

export const ProjectCard = ({ project }: ProjectProps) => {
  return (
    <Card>
      <Title>{project.name}</Title>
          <Description>{project.description ? project.description.slice(0, 220) + (project.description.length > 220 ? '...' : '') : ''}</Description>
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
