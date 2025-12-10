import styled from "styled-components";
import { Link } from "react-router-dom";
import type { Project } from "../types";
import { colors, shadows, fonts, spacing, borderRadius, transitions } from "../../../shared/styles/tokens";
import { baseUrl } from "../../../shared/services/base";

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
  overflow: hidden;

  border-left: 4px solid ${colors.accentBorder};
  &:hover { transform: translateY(-6px); box-shadow: ${shadows.lg} }
`;

const Title = styled.h3`
  margin: 0;
  font-size: ${fonts.size.xl};
  letter-spacing: 0.2px;
  color: ${colors.textPrimary};
  font-weight: ${fonts.weight.semibold};
  word-break: break-all;
`;

const Description = styled.p`
  margin: 0;
  font-size: ${fonts.size.base};
  color: ${colors.textSecondary};
  line-height: 1.45;
  word-break: break-all;
`;

const AuthorDate = styled.div`
  font-size: ${fonts.size.xs};
  color: ${colors.muted};
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: auto;
`;

const AuthorLink = styled(Link)`
  color: ${colors.primary};
  text-decoration: none;
  cursor: pointer;
  transition: color ${transitions.base};

  &:hover {
    color: ${colors.primaryDark};
    text-decoration: underline;
  }
`;

const Rating = styled.span`
  font-size: ${fonts.size.sm};
  color: ${colors.primary};
  font-weight: ${fonts.weight.medium};
`;

const ImagesContainer = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  justify-content: flex-start;
`;

const Image = styled.img`
  width: 130px;
  height: 75px;
  border-radius: ${borderRadius.sm};
  object-fit: cover;
  border: 1px solid ${colors.accentBorderDark};
  box-shadow: 0 6px 18px rgba(2,6,23,0.04);
`;

const ExternalUrl = styled.div`
  font-size: ${fonts.size.sm};
  color: ${colors.primary};
  display: flex;
  align-items: center;
  margin-top: ${spacing.xs};

  a {
    color: ${colors.primary};
    text-decoration: none;
    cursor: pointer;
    transition: color ${transitions.base};
    display: flex;
    align-items: center;
    gap: ${spacing.xs};

    &:hover {
      color: ${colors.primaryDark};
      text-decoration: underline;
    }
  }
`;

interface ProjectCardProps {
  project: Project;
}

const formatDate = (dateString: string): string => {
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  } catch {
    return '';
  }
};

export const ProjectCard = ({ project }: ProjectCardProps) => {
  const renderImages = () => {
    const imagePaths = project.imagePaths || [];
    return imagePaths.slice(0, 6).map((path, idx) => (
      <Image
        key={idx}
        src={`${baseUrl}/api/Projects/${project.id}/${path}`}
        alt={`Project image ${idx + 1}`}
      />
    ));
  };

  return (
    <Link to={`/projects/${project.id}`} style={{ textDecoration: 'none', color: 'inherit' }}>
      <Card>
        <Title>{project.name}</Title>
        <Description>{project.description ? project.description.slice(0, 220) + (project.description.length > 220 ? '...' : '') : ''}</Description>
        {project.externalUrl && project.externalUrl.trim() && (
          <ExternalUrl>
            <a href={project.externalUrl} target="_blank" rel="noopener noreferrer" onClick={(e) => e.stopPropagation()}>
              🔗 External Link: {project.externalUrl}
            </a>
          </ExternalUrl>
        )}
        <ImagesContainer>
          {renderImages()}
        </ImagesContainer>
        <AuthorDate>
          <AuthorLink to={`/profile/${project.author}`} onClick={(e) => e.stopPropagation()}>
            {project.author ? project.author.substring(0, 8) : 'Unknown'}
          </AuthorLink>
          <Rating>☆ {project.averageRating?.toFixed(1) ?? 'N/A'}</Rating>
          <span>{project.creationDate && formatDate(project.creationDate)}</span>
        </AuthorDate>
      </Card>
    </Link>
  );
};
