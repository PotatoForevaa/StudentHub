import styled from "styled-components";
import { Link, useNavigate } from "react-router-dom";
import type { Project } from "../types";
import { colors, shadows, fonts, spacing, borderRadius, transitions } from "../../../shared/styles/tokens";
import { formatDate } from "../../../shared/utils/date";
import { ProjectImages } from "./ProjectImages";
import { API_BASE_URL } from "../../../shared/services/base";

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
  cursor: pointer;
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
  display: flex;
  align-items: center;
  gap: ${spacing.sm};

  &:hover {
    color: ${colors.primaryDark};
    text-decoration: underline;
  }
`;

const AuthorAvatar = styled.img`
  width: 24px;
  height: 24px;
  border-radius: 50%;
  object-fit: cover;
  border: 1px solid ${colors.accentBorderLight};
`;

const Rating = styled.span`
  font-size: ${fonts.size.sm};
  color: ${colors.primary};
  font-weight: ${fonts.weight.medium};
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

export const ProjectCard = ({ project }: ProjectCardProps) => {
  const navigate = useNavigate();

  return (
    <Card onClick={() => navigate(`/projects/${project.id}`)}>
        <Title>{project.name}</Title>
        <Description>{project.description ? project.description.slice(0, 220) + (project.description.length > 220 ? '...' : '') : ''}</Description>
        {project.externalUrl && project.externalUrl.trim() && (
          <ExternalUrl>
            <a href={project.externalUrl} target="_blank" rel="noopener noreferrer" onClick={(e) => e.stopPropagation()}>
              🔗 External Link: {project.externalUrl}
            </a>
          </ExternalUrl>
        )}
        <ProjectImages
          projectId={project.id}
          imagePaths={project.imagePaths || []}
          maxImages={6}
        />
        <AuthorDate>
          <AuthorLink to={`/${project.authorUsername}`} onClick={(e) => e.stopPropagation()}>
            <AuthorAvatar
              src={`${API_BASE_URL}/${project.authorProfilePicturePath}`}
              alt={`${project.authorName} avatar`}
              onError={(e) => {
                const target = e.target as HTMLImageElement;
                target.src = `${API_BASE_URL}/api/users/by-username/admin/profile-picture`;
              }}
            />
            {project.authorName ? project.authorName.substring(0, 8) : 'Unknown'}
          </AuthorLink>
          <Rating>☆ {project.averageRating?.toFixed(1) ?? 'N/A'}</Rating>
          <span>{project.creationDate && formatDate(project.creationDate)}</span>
        </AuthorDate>
    </Card>
  );
};
