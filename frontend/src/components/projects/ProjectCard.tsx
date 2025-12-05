import styled from "styled-components";
import type { Project } from "../../types/Project";
import { baseUrl } from "../../services/api/base";
import { useEffect, useState } from 'react';
import userService from '../../services/api/userService';
import { colors, shadows, fonts, spacing, borderRadius, transitions } from "../../styles/tokens";

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
  const [authorName, setAuthorName] = useState<string | null>(null);

  useEffect(() => {
    let mounted = true;
    const fetchAuthor = async () => {
      try {
        if (!project.authorId) return;
        const res = await userService.getById(project.authorId);
        if (mounted && res && res.isSuccess && res.data) {
          setAuthorName(res.data.fullName || res.data.username || null);
        }
      } catch {
        // ignore
      }
    };
    fetchAuthor();
    return () => {
      mounted = false;
    };
  }, [project.authorId]);

  return (
    <Card>
      <Title>{project.name}</Title>
      <Description>{project.description ? project.description.slice(0, 220) + (project.description.length > 220 ? '...' : '') : ''}</Description>
      <ImagesContainer>
        {project.imagePaths?.map((img, idx) => (
          <Image key={idx} src={`${baseUrl}/api/Projects/${project.id}/${img}`} alt={`Project image ${idx + 1}`} />
        ))}
      </ImagesContainer>
      <AuthorDate>
        <span>{authorName ? `By ${authorName}` : project.authorId ? `By ${project.authorId.substring(0, 8)}...` : 'Unknown Author'}</span>
        <span>{project.createdAt && formatDate(project.createdAt)}</span>
      </AuthorDate>
    </Card>
  );
};
