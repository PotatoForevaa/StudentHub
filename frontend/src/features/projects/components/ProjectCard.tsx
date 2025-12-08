import { useContext, useEffect, useState } from "react";
import styled from "styled-components";
import type { Project } from "../types";
import { ProjectContext } from "../context/ProjectContext";
import { projectService } from "../services/projectService";
import { colors, shadows, fonts, spacing, borderRadius, transitions } from "../../../shared/styles/tokens";

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
  const { getProjectImages } = useContext(ProjectContext);
  const [imageDataUrls, setImageDataUrls] = useState<string[]>([]);
  const [loadingImages, setLoadingImages] = useState(true);

  useEffect(() => {
    const loadImages = async () => {
      setLoadingImages(true);
      try {
        // Get image paths for this project
        const imagePaths = await getProjectImages(project.id);

        // Fetch each image and convert to data URL
        const dataUrls: string[] = [];
        for (const path of imagePaths || []) {
          try {
            const response = await projectService.getImage(project.id, path);
            if (response.ok) {
              const blob = await response.blob();
              const dataUrl = URL.createObjectURL(blob);
              dataUrls.push(dataUrl);
            }
          } catch (error) {
            console.error(`Failed to load image ${path}:`, error);
          }
        }

        setImageDataUrls(dataUrls);
      } catch (error) {
        console.error('Failed to load project images:', error);
        setImageDataUrls([]);
      } finally {
        setLoadingImages(false);
      }
    };

    loadImages();
  }, [project.id, getProjectImages]);

  return (
    <Card>
      <Title>{project.name}</Title>
      <Description>{project.description ? project.description.slice(0, 220) + (project.description.length > 220 ? '...' : '') : ''}</Description>
      <ImagesContainer>
        {loadingImages ? (
          <div>Loading images...</div>
        ) : (
          imageDataUrls.slice(0, 3).map((dataUrl, idx) => (
            <Image key={idx} src={dataUrl} alt={`Project image ${idx + 1}`} />
          ))
        )}
      </ImagesContainer>
      <AuthorDate>
        <span>{project.author ? project.author.substring(0, 8) : 'Unknown'}</span>
        <span>{project.creationDate && formatDate(project.creationDate)}</span>
      </AuthorDate>
    </Card>
  );
};
