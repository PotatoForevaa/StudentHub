import styled from "styled-components";
import { colors, borderRadius } from "../../../shared/styles/tokens";
import { projectService } from "../services/projectService";

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

interface ProjectImagesProps {
  projectId: string;
  imagePaths: string[];
  maxImages?: number;
}

export const ProjectImages = ({ projectId, imagePaths, maxImages = 6 }: ProjectImagesProps) => {
  if (!imagePaths || imagePaths.length === 0) {
    return null;
  }

  const displayImages = imagePaths.slice(0, maxImages);

  return (
    <ImagesContainer>
      {displayImages.map((path, idx) => (
        <Image
          key={idx}
          src={projectService.getProjectImagePath(projectId, path)}
          alt={`Project image ${idx + 1}`}
        />
      ))}
    </ImagesContainer>
  );
};
