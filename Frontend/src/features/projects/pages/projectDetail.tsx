import { useEffect, useState, useCallback } from "react";
import { useParams, Link } from "react-router-dom";
import styled from "styled-components";
import { ProjectProvider } from "../context/ProjectContext";
import { Container } from "../../../shared/components/Container";
import { projectService } from "../services/projectService";
import type { Project, Comment } from "../types";
import { colors, shadows, fonts, spacing, borderRadius, transitions } from "../../../shared/styles/tokens";
import { baseUrl } from "../../../shared/services/base";

const ProjectDetailContainer = styled.div`
  display: flex;
  flex-direction: column;
  gap: ${spacing.lg};
  max-width: 800px;
  width: 100%;
`;

const ProjectHeader = styled.div`
  border-radius: ${borderRadius.lg};
  padding: ${spacing.lg};
  background: ${colors.surface};
  box-shadow: ${shadows.sm};
`;

const ProjectTitle = styled.h1`
  margin: 0 0 ${spacing.md} 0;
  font-size: ${fonts.size['2xl']};
  font-weight: ${fonts.weight.bold};
  color: ${colors.textPrimary};
`;

const ProjectMeta = styled.div`
  display: flex;
  justify-content: space-between;
  font-size: ${fonts.size.sm};
  color: ${colors.textSecondary};
  margin-bottom: ${spacing.md};
`;

const AverageRating = styled.div`
  font-size: ${fonts.size.xl};
  font-weight: ${fonts.weight.bold};
  color: ${colors.primary};
  margin-bottom: ${spacing.sm};
  text-align: center;
`;

const ScoreSectionContent = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
`;

const ProjectDescription = styled.p`
  margin: ${spacing.md} 0;
  font-size: ${fonts.size.base};
  line-height: 1.6;
  color: ${colors.textPrimary};
`;

const ImagesGrid = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: ${spacing.md};
  margin: ${spacing.lg} 0;
  align-items: start;
`;

const FullImage = styled.img`
  width: 100%;
  height: auto;
  max-height: 300px;
  object-fit: contain;
  border-radius: ${borderRadius.md};
  border: 1px solid ${colors.accentBorder};
  box-shadow: ${shadows.sm};
  cursor: pointer;
  transition: transform ${transitions.base}, box-shadow ${transitions.base};
  display: block;

  &:hover {
    transform: scale(1.03);
    box-shadow: ${shadows.md};
  }
`;

const CommentsSection = styled.div`
  background: ${colors.surface};
  border-radius: ${borderRadius.lg};
  padding: ${spacing.lg};
  box-shadow: ${shadows.sm};
`;

const ScoreSection = styled.div`
  background: ${colors.surface};
  border-radius: ${borderRadius.lg};
  padding: ${spacing.lg};
  box-shadow: ${shadows.sm};
`;

const CommentsTitle = styled.h3`
  margin: 0 0 ${spacing.lg} 0;
  font-size: ${fonts.size.xl};
  font-weight: ${fonts.weight.semibold};
  color: ${colors.textPrimary};
`;

const BackLink = styled(Link)`
  display: inline-block;
  margin-bottom: ${spacing.lg};
  padding: ${spacing.sm} ${spacing.md};
  background: ${colors.primary};
  color: ${colors.white};
  text-decoration: none;
  border-radius: ${borderRadius.md};
  transition: background ${transitions.base};

  &:hover {
    background: ${colors.primaryDark};
  }
`;

function ProjectDetailContent() {
  const { id } = useParams<{ id: string }>();
  const [project, setProject] = useState<Project | null>(null);
  const [imagePaths, setImagePaths] = useState<string[]>([]);
  const [comments, setComments] = useState<Comment[]>([]);
  const [loading, setLoading] = useState(true);
  const [newComment, setNewComment] = useState('');
  const [newScore, setNewScore] = useState(0);

  const loadProject = useCallback(async () => {
    try {
      const result = await projectService.getProject(id!);
      if (result.isSuccess) {
        setProject(result.data || null);
      }
    } catch (error) {
      console.error("Failed to load project", error);
    }
  }, [id]);

  const loadImageList = useCallback(async () => {
    try {
      const result = await projectService.getImageList(id!);
      if (result.isSuccess) {
        setImagePaths((result.data as string[]) || []);
      }
    } catch (error) {
      console.error("Failed to load image list", error);
    }
  }, [id]);

  const loadComments = useCallback(async () => {
    try {
      const result = await projectService.getComments(id!);
      if (result.isSuccess) {
        setComments(result.data || []);
      }
      setLoading(false);
    } catch (error) {
      console.error("Failed to load comments", error);
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    if (id) {
      loadProject();
      loadImageList();
      loadComments();
    }
  }, [id, loadProject, loadImageList, loadComments]);

  const formatDate = (dateString: string) => {
    try {
      return new Date(dateString).toLocaleDateString();
    } catch {
      return dateString;
    }
  };

  const handleAddComment = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newComment.trim()) return;

    try {
      const result = await projectService.addComment(id!, { content: newComment });
      if (result.isSuccess) {
        setComments(prev => [result.data!, ...prev]);
        setNewComment('');
      }
    } catch (error) {
      console.error("Failed to add comment", error);
    }
  };

  if (loading) {
    return <Container>Загрузка...</Container>;
  }

  if (!project) {
    return <Container>Проект не найден</Container>;
  }

  return (
    <Container>
      <ProjectDetailContainer>
        <BackLink to="/projects">← Назад к проектам</BackLink>

        <ProjectHeader>
          <ProjectTitle>{project.name}</ProjectTitle>
          <ProjectMeta>
            <span>Автор: <AuthorLink to={`/profile/${project.author}`}>{project.author}</AuthorLink></span>
            <span>{formatDate(project.creationDate)}</span>
          </ProjectMeta>
          {project.description && (
            <ProjectDescription>{project.description}</ProjectDescription>
          )}
          {project.externalUrl && (
            <a href={project.externalUrl} target="_blank" rel="noopener noreferrer">
              Посмотреть внешнюю ссылку
            </a>
          )}
        </ProjectHeader>

        {imagePaths.length > 0 && (
          <div>
            <h3>Изображения</h3>
            <ImagesGrid>
              {imagePaths.map((path, idx) => (
                <FullImage
                  key={idx}
                  src={`${baseUrl}/api/Projects/${project.id}/${path}`}
                  alt={`Изображение проекта ${idx + 1}`}
                />
              ))}
            </ImagesGrid>
          </div>
        )}

        <ScoreSection>
          <CommentsTitle>Оценить проект</CommentsTitle>
          <ScoreSectionContent>
            <AverageRating>
              Средняя оценка: {project.averageRating.toFixed(1)} / 5.0
            </AverageRating>
            <StarSelector>
              {[1, 2, 3, 4, 5].map(star => (
                <StarButton
                  key={star}
                  type="button"
                  selected={newScore >= star}
                  onClick={async () => {
                    setNewScore(star);
                    try {
                      const result = await projectService.addScore(id!, { score: star });
                      if (result.isSuccess) {
                        setProject(prev => prev ? { ...prev, averageRating: result.data ?? 0 } : prev);
                      } else {
                        alert('Не удалось отправить оценку');
                        setNewScore(0);
                      }
                    } catch (error) {
                      console.error("Failed to add score", error);
                      alert('Не удалось отправить оценку');
                      setNewScore(0);
                    }
                  }}
                >
                  ★
                </StarButton>
              ))}
            </StarSelector>
          </ScoreSectionContent>
        </ScoreSection>

        <CommentsSection>
          <CommentsTitle>Комментарии ({comments.length})</CommentsTitle>
          <Form onSubmit={handleAddComment}>
            <TextArea
              value={newComment}
              onChange={(e) => setNewComment(e.target.value)}
              placeholder="Напишите комментарий..."
            />
            <Button type="submit" disabled={!newComment.trim()}>
              Опубликовать комментарий
            </Button>
          </Form>
          {comments.map(comment => (
            <CommentItem key={comment.id} comment={comment} />
          ))}
        </CommentsSection>
      </ProjectDetailContainer>
    </Container>
  );
}

function CommentItem({ comment }: { comment: Comment }) {
  const formatDate = (dateString: string) => {
    try {
      return new Date(dateString).toLocaleDateString();
    } catch {
      return dateString;
    }
  };

  return (
    <CommentContainer>
      <CommentHeader>
        {comment.authorProfilePicturePath && (
          <ProfilePic src={`${baseUrl}/api/Users/ProfilePicture/${comment.authorUsername}`} alt={comment.authorUsername || ''} />
        )}
        <CommentUserLink to={`/profile/${comment.authorUsername}`}>
          {comment.authorUsername || 'Аноним'}
        </CommentUserLink>
        {comment.userScore && <SmallStars>{Array.from({ length: comment.userScore }, () => '★').join('')}</SmallStars>}
        <CommentDate>{formatDate(comment.createdAt)}</CommentDate>
      </CommentHeader>
      <CommentText>{comment.content}</CommentText>
    </CommentContainer>
  );
}

const CommentContainer = styled.div`
  border-bottom: 1px solid ${colors.accentBorder};
  padding-bottom: ${spacing.md};
  margin-bottom: ${spacing.md};

  &:last-child {
    border-bottom: none;
    margin-bottom: 0;
  }
`;

const CommentHeader = styled.div`
  display: flex;
  align-items: center;
  gap: ${spacing.sm};
  margin-bottom: ${spacing.sm};
`;

const ProfilePic = styled.img`
  width: 32px;
  height: 32px;
  border-radius: 50%;
  object-fit: cover;
`;



const CommentDate = styled.span`
  margin-left: auto;
  font-size: ${fonts.size.sm};
  color: ${colors.textSecondary};
`;

const CommentUserLink = styled(Link)`
  color: ${colors.primary};
  text-decoration: none;
  cursor: pointer;
  transition: color ${transitions.base};

  &:hover {
    color: ${colors.primaryDark};
    text-decoration: underline;
  }
`;

const SmallStars = styled.span`
  color: ${colors.primary};
  margin-left: ${spacing.xs};
`;

const CommentText = styled.p`
  margin: 0;
  color: ${colors.textPrimary};
  line-height: 1.5;
  word-break: break-all;
`;

const AuthorLink = styled(Link)`
  color: ${colors.textPrimary};
  text-decoration: none;
  cursor: pointer;
  transition: color ${transitions.base};

  &:hover {
    color: ${colors.primary};
  }
`;

const Form = styled.form`
  margin-bottom: ${spacing.lg};
  padding: ${spacing.md};
  background: ${colors.bg};
  border-radius: ${borderRadius.md};
`;

const TextArea = styled.textarea`
  width: 100%;
  padding: ${spacing.sm};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.sm};
  font-family: inherit;
  resize: vertical;
  margin-bottom: ${spacing.sm};
  min-height: 80px;

  &:focus {
    outline: none;
    border-color: ${colors.primary};
  }
`;

const Button = styled.button`
  background: ${colors.primary};
  color: ${colors.white};
  border: none;
  padding: ${spacing.sm} ${spacing.md};
  border-radius: ${borderRadius.sm};
  cursor: pointer;
  transition: background ${transitions.base};
  font-weight: ${fonts.weight.semibold};

  &:hover {
    background: ${colors.primaryDark};
  }

  &:disabled {
    background: ${colors.muted};
    cursor: not-allowed;
  }
`;

const StarSelector = styled.div`
  display: flex;
  width: 100%;
  justify-content: space-between;
  align-items: center;
  margin-bottom: ${spacing.sm};
`;

const StarButton = styled.button<{ selected: boolean }>`
  background: none;
  border: none;
  font-size: 4rem;
  cursor: pointer;
  color: ${props => props.selected ? colors.primary : colors.muted};
  padding: 0;
  margin: 0;
  transition: color ${transitions.base};

  &:hover {
    color: ${colors.primary};
  }
`;

export const ProjectDetail = () => (
  <ProjectProvider>
    <ProjectDetailContent />
  </ProjectProvider>
);
