import { useEffect, useState, useCallback, useContext } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";
import styled from "styled-components";
import { ProjectProvider } from "../context/ProjectContext";
import { Container } from "../../../shared/components/Container";
import { projectService } from "../services/projectService";
import { ProjectUpdateForm } from "../components/ProjectUpdateForm";
import { Pagination } from "../components/Pagination";
import type { Project, Comment, CriterionDto } from "../types";
import { colors, shadows, fonts, spacing, borderRadius, transitions } from "../../../shared/styles/tokens";
import userService from "../../../shared/services/userService";
import { AuthContext } from "../../auth/context/AuthContext";
import { isAdmin, isTeacher } from "../../../shared/utils/roles";
import { adminService } from "../../admin/services/adminService";
import { moderationService } from "../../moderation/services/moderationService";

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
  margin: 0;
  font-size: ${fonts.size['2xl']};
  font-weight: ${fonts.weight.bold};
  color: ${colors.textPrimary};
`;

const EditButton = styled.button`
  background: ${colors.gray400};
  color: ${colors.white};
  border: none;
  padding: ${spacing.sm} ${spacing.md};
  border-radius: ${borderRadius.sm};
  cursor: pointer;
  transition: background ${transitions.base};
  font-weight: ${fonts.weight.semibold};

  &:hover {
    background: ${colors.gray500};
  }
`;

const DeleteButton = styled.button`
  background: #dc3545;
  color: ${colors.white};
  border: none;
  padding: ${spacing.sm} ${spacing.md};
  border-radius: ${borderRadius.sm};
  cursor: pointer;
  transition: background ${transitions.base};
  font-weight: ${fonts.weight.semibold};

  &:hover {
    background: #c82333;
  }
`;

const ProjectMeta = styled.div`
  display: flex;
  justify-content: space-between;
  font-size: ${fonts.size.sm};
  color: ${colors.textSecondary};
  margin-bottom: ${spacing.md};
`;

const ButtonContainer = styled.div`
  display: flex;
  gap: ${spacing.sm};
  align-items: center;
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

const CriteriaScoreSection = styled.div`
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

const SectionTitle = styled.h3`
  margin: 0 0 ${spacing.md} 0;
  font-size: ${fonts.size.lg};
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

const ScoreTable = styled.table`
  width: 100%;
  border-collapse: collapse;
  margin-bottom: ${spacing.md};
`;

const ScoreTh = styled.th`
  text-align: left;
  padding: ${spacing.sm};
  border-bottom: 1px solid ${colors.accentBorder};
  color: ${colors.textSecondary};
  font-size: ${fonts.size.sm};
  font-weight: ${fonts.weight.semibold};
`;

const ScoreTd = styled.td`
  padding: ${spacing.sm};
  border-bottom: 1px solid ${colors.accentBorderLight};
  font-size: ${fonts.size.sm};
  color: ${colors.textPrimary};
`;

const ScoreValue = styled.span`
  font-weight: ${fonts.weight.bold};
  color: ${colors.primary};
  font-size: ${fonts.size.base};
`;

const TeacherScoreForm = styled.form`
  margin-top: ${spacing.md};
  padding: ${spacing.md};
  background: ${colors.bg};
  border-radius: ${borderRadius.md};
  display: flex;
  flex-direction: column;
  gap: ${spacing.md};
`;

const CriterionScoreRow = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: ${spacing.sm};
  align-items: center;
  padding: ${spacing.sm};
  border-bottom: 1px solid ${colors.accentBorderLight};
`;

const CriterionName = styled.span`
  flex: 1;
  min-width: 150px;
  font-size: ${fonts.size.sm};
  color: ${colors.textPrimary};
  font-weight: ${fonts.weight.medium};
`;

const ScoreInput = styled.input`
  width: 60px;
  padding: ${spacing.xs} ${spacing.sm};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.sm};
  font-size: ${fonts.size.sm};
  text-align: center;

  &:focus {
    outline: none;
    border-color: ${colors.primary};
  }
`;

const CommentInput = styled.input`
  flex: 1;
  min-width: 200px;
  padding: ${spacing.xs} ${spacing.sm};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.sm};
  font-size: ${fonts.size.sm};

  &:focus {
    outline: none;
    border-color: ${colors.primary};
  }
`;

const EmptyScores = styled.p`
  color: ${colors.textSecondary};
  font-size: ${fonts.size.sm};
  font-style: italic;
`;

function ProjectDetailContent() {
  const { id } = useParams<{ id: string }>();
  const { user } = useContext(AuthContext);
  const navigate = useNavigate();
  const [project, setProject] = useState<Project | null>(null);
  const [imagePaths, setImagePaths] = useState<string[]>([]);
  const [comments, setComments] = useState<Comment[]>([]);
  const [commentsPage, setCommentsPage] = useState(1);
  const [commentsTotalPages, setCommentsTotalPages] = useState(0);
  const [loading, setLoading] = useState(true);
  const [newComment, setNewComment] = useState('');
  const [newScore, setNewScore] = useState(0);
  const [isEditing, setIsEditing] = useState(false);

  // Teacher scoring state
  const [isTeacherUser, setIsTeacherUser] = useState(false);
  const [teacherCategoryId, setTeacherCategoryId] = useState("");
  const [teacherCategoryName, setTeacherCategoryName] = useState("");
  const [criteria, setCriteria] = useState<CriterionDto[]>([]);
  const [scores, setScores] = useState<Record<string, { score: number; comment: string }>>({});
  const [submittingScores, setSubmittingScores] = useState(false);
  const [criterionScores, setCriterionScores] = useState<Project['criterionScores']>([]);

  // For the score table category column - we now use categoryName from the backend DTO
  // (the CriterionScoreDto now includes categoryName)

  const loadProject = useCallback(async () => {
    try {
      const result = await projectService.getProject(id!);
      if (result.isSuccess) {
        setProject(result.data || null);
        setCriterionScores(result.data?.criterionScores || []);
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
      const result = await projectService.getComments(id!, commentsPage, 10);
      if (result.isSuccess) {
        setComments(result.data?.items || []);
        setCommentsTotalPages(result.data?.totalPages || 0);
      }
      setLoading(false);
    } catch (error) {
      console.error("Failed to load comments", error);
      setLoading(false);
    }
  }, [id, commentsPage]);

  useEffect(() => {
    if (id) {
      loadProject();
      loadImageList();
      loadComments();
    }
  }, [id, loadProject, loadImageList, loadComments]);

  // Auto-detect category from the project (project now has exactly 1 category)
  useEffect(() => {
    if (project && project.categories && project.categories.length > 0) {
      const cat = project.categories[0];
      setTeacherCategoryId(cat.id);
      setTeacherCategoryName(cat.name);
    }
  }, [project]);

  // Load criteria when category is detected from project
  useEffect(() => {
    if (teacherCategoryId) {
      projectService.getCriteriaByCategory(teacherCategoryId).then(result => {
        if (result.isSuccess && result.data) {
          setCriteria(result.data);
          setScores({});
        }
      });
    } else {
      setCriteria([]);
      setScores({});
    }
  }, [teacherCategoryId]);

  useEffect(() => {
    if (user) {
      setIsTeacherUser(isTeacher(user) || isAdmin(user));
    }
  }, [user]);

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
        setNewComment('');
        setCommentsPage(1);
        await loadComments();
      }
    } catch (error) {
      console.error("Failed to add comment", error);
    }
  };

  const handleAddScore = async (e: React.FormEvent) => {
    e.preventDefault();
    if (newScore === 0) return;

    try {
      const result = await projectService.addScore(id!, { score: newScore });
      if (result.isSuccess) {
        await loadProject();
        setNewScore(0);
      }
    } catch (error) {
      console.error("Failed to add score", error);
    }
  };

  const handleStartEdit = () => {
    setIsEditing(true);
  };

  const handleCancelEdit = () => {
    setIsEditing(false);
  };

  const handleDeleteProject = async () => {
    if (!id) return;

    const confirmed = window.confirm('Вы уверены, что хотите удалить этот проект? Это действие нельзя отменить.');
    if (!confirmed) return;

    try {
      const result = await projectService.deleteProject(id);
      if (result.isSuccess) {
        navigate('/projects');
      } else {
        alert('Не удалось удалить проект');
      }
    } catch (error) {
      console.error('Failed to delete project', error);
      alert('Произошла ошибка при удалении проекта');
    }
  };

  const handleCriterionScoreChange = (criterionId: string, value: string) => {
    const numValue = parseInt(value, 10);
    if (value === '' || (numValue >= 1 && numValue <= 10)) {
      setScores(prev => ({
        ...prev,
        [criterionId]: { ...prev[criterionId], score: value === '' ? 0 : numValue }
      }));
    }
  };

  const handleCriterionCommentChange = (criterionId: string, comment: string) => {
    setScores(prev => ({
      ...prev,
      [criterionId]: { ...prev[criterionId], comment }
    }));
  };

  const handleSubmitCriterionScores = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!id || !teacherCategoryId) return;

    const scoresData = criteria
      .filter(c => scores[c.id] && scores[c.id].score > 0)
      .map(c => ({
        criterionId: c.id,
        score: scores[c.id].score,
        comment: scores[c.id].comment || undefined,
      }));

    if (scoresData.length === 0) {
      alert('Выберите хотя бы одну оценку');
      return;
    }

    setSubmittingScores(true);
    try {
      const result = await projectService.submitScores(id, { scores: scoresData });
      if (result.isSuccess) {
        await loadProject();
        setScores({});
        alert('Оценки успешно сохранены');
      } else {
        alert('Не удалось сохранить оценки');
      }
    } catch (error) {
      console.error("Failed to submit scores", error);
      alert('Ошибка при сохранении оценок');
    } finally {
      setSubmittingScores(false);
    }
  };

  const isAuthor = user && project && user.username === project.authorUsername;
  const userIsAdmin = isAdmin(user);

  const handleMarkCommentToxic = async (commentId: string) => {
    try {
      const result = await adminService.markCommentToxic(commentId);
      if (result.isSuccess) {
        setComments(prev => prev.filter(comment => comment.id !== commentId));
      }
    } catch (error) {
      console.error("Failed to mark comment toxic", error);
    }
  };

  const handleAppealComment = async (commentId: string) => {
    const confirmed = window.confirm('Отправить апелляцию на этот комментарий?');
    if (!confirmed) return;

    try {
      const result = await moderationService.submitAppeal(commentId, 'Я считаю, что этот комментарий был ошибочно помечен.');
      if (result.isSuccess) {
        await loadComments();
      }
    } catch (error) {
      console.error("Failed to submit appeal", error);
    }
  };

  const handleReportComment = async (commentId: string) => {
    if (!id) return;

    try {
      const result = await projectService.reportComment(id, commentId);
      if (result.isSuccess) {
        await loadComments();
      }
    } catch (error) {
      console.error("Failed to report comment", error);
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
          {isEditing && project ? (
            <ProjectUpdateForm
              project={project}
              onSuccess={() => {
                setIsEditing(false);
                loadProject();
                loadImageList();
              }}
              onCancel={handleCancelEdit}
            />
          ) : (
            <>
              <ProjectTitle>{project.name}</ProjectTitle>
              <ProjectMeta>
                <span>Автор: <AuthorLink to={`/users/${project.authorUsername}`}>{project.authorName}</AuthorLink></span>
                <span>{formatDate(project.creationDate)}</span>
                {isAuthor && (
                  <ButtonContainer>
                    <EditButton onClick={handleStartEdit}>
                      Редактировать
                    </EditButton>
                    <DeleteButton onClick={handleDeleteProject}>
                      Удалить
                    </DeleteButton>
                  </ButtonContainer>
                )}
              </ProjectMeta>
              {(project.categories && project.categories.length > 0) && (
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px', marginBottom: '12px' }}>
                  {project.categories.map((cat) => (
                    <span key={cat.id} style={{ background: '#dbeafe', color: '#1e40af', padding: '4px 12px', borderRadius: '6px', fontSize: '13px', fontWeight: 500 }}>
                      📁 {cat.name}
                    </span>
                  ))}
                </div>
              )}
              {(project.tags && project.tags.length > 0) && (
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px', marginBottom: '12px' }}>
                  {project.tags.map((tag) => (
                    <span key={tag.id} style={{ background: '#f3f4f6', color: '#6b7280', padding: '4px 12px', borderRadius: '6px', fontSize: '13px', fontWeight: 400 }}>
                      🏷️ {tag.name}
                    </span>
                  ))}
                </div>
              )}
              {project.description && (
                <ProjectDescription>{project.description}</ProjectDescription>
              )}
              {project.externalUrl && (
                <a href={project.externalUrl} target="_blank" rel="noopener noreferrer">
                  Посмотреть внешнюю ссылку
                </a>
              )}
            </>
          )}
        </ProjectHeader>

        {imagePaths.length > 0 && (
          <div>
            <h3>Изображения</h3>
            <ImagesGrid>
              {imagePaths.map((path, idx) => (
                <FullImage
                  key={idx}
                  src={projectService.getProjectImagePath(project.id, path)}
                  alt={`Изображение проекта ${idx + 1}`}
                />
              ))}
            </ImagesGrid>
          </div>
        )}

        {/* Criteria scores section - visible to everyone */}
        <CriteriaScoreSection>
          <SectionTitle>Оценки преподавателей</SectionTitle>
          {criterionScores && criterionScores.length > 0 ? (
            <ScoreTable>
              <thead>
                <tr>
                  <ScoreTh>Критерий</ScoreTh>
                  <ScoreTh>Категория</ScoreTh>
                  <ScoreTh>Оценка</ScoreTh>
                  <ScoreTh>Комментарий</ScoreTh>
                  <ScoreTh>Преподаватель</ScoreTh>
                  <ScoreTh>Дата</ScoreTh>
                </tr>
              </thead>
              <tbody>
                {criterionScores.map((cs, idx) => (
                  <tr key={idx}>
                    <ScoreTd>{cs.criterionName}</ScoreTd>
                    <ScoreTd>{cs.categoryName || ''}</ScoreTd>
                    <ScoreTd><ScoreValue>{cs.score}/10</ScoreValue></ScoreTd>
                    <ScoreTd>{cs.comment || '—'}</ScoreTd>
                    <ScoreTd>{cs.teacherName}</ScoreTd>
                    <ScoreTd>{formatDate(cs.createdAt)}</ScoreTd>
                  </tr>
                ))}
              </tbody>
            </ScoreTable>
          ) : (
            <EmptyScores>Оценки ещё не выставлены.</EmptyScores>
          )}
        </CriteriaScoreSection>

        {/* Teacher scoring form - only for teachers */}
        {isTeacherUser && (
          <CriteriaScoreSection>
            <SectionTitle>Выставить оценки</SectionTitle>
            {teacherCategoryId ? (
              <TeacherScoreForm onSubmit={handleSubmitCriterionScores}>
                <div>
                  <label style={{ display: 'block', marginBottom: '4px', fontSize: '13px', color: colors.textSecondary, fontWeight: 500 }}>
                    Категория
                  </label>
                  <div style={{ padding: '8px 12px', background: colors.white, border: `1px solid ${colors.accentBorder}`, borderRadius: borderRadius.sm, fontSize: fonts.size.sm, color: colors.textPrimary, maxWidth: '300px' }}>
                    {teacherCategoryName}
                  </div>
                </div>

                {criteria.length === 0 && (
                  <EmptyScores>Нет критериев для этой категории.</EmptyScores>
                )}

                {criteria.map(criterion => (
                  <CriterionScoreRow key={criterion.id}>
                    <CriterionName>{criterion.name}</CriterionName>
                    <ScoreInput
                      type="number"
                      min={1}
                      max={10}
                      placeholder="1-10"
                      value={scores[criterion.id]?.score || ''}
                      onChange={e => handleCriterionScoreChange(criterion.id, e.target.value)}
                    />
                    <CommentInput
                      type="text"
                      placeholder="Комментарий (необязательно)"
                      value={scores[criterion.id]?.comment || ''}
                      onChange={e => handleCriterionCommentChange(criterion.id, e.target.value)}
                    />
                  </CriterionScoreRow>
                ))}

                {criteria.length > 0 && (
                  <Button type="submit" disabled={submittingScores}>
                    {submittingScores ? 'Сохранение...' : 'Сохранить оценки'}
                  </Button>
                )}
              </TeacherScoreForm>
            ) : (
              <EmptyScores>У проекта нет категории, по которой можно выставить оценки.</EmptyScores>
            )}
          </CriteriaScoreSection>
        )}

        <ScoreSection>
          <ScoreSectionContent>
            <AverageRating>{project.averageRating.toFixed(1)} ★</AverageRating>
            <Form onSubmit={handleAddScore}>
              <StarSelector>
                {[1, 2, 3, 4, 5].map((star) => (
                  <StarButton
                    key={star}
                    selected={newScore >= star}
                    onClick={() => setNewScore(star)}
                  >
                    ★
                  </StarButton>
                ))}
              </StarSelector>              
            </Form>
          </ScoreSectionContent>
        </ScoreSection>

        <CommentsSection>
          <CommentsTitle>Комментарии</CommentsTitle>
          <Form onSubmit={handleAddComment}>
            <TextArea
              value={newComment}
              onChange={(e) => setNewComment(e.target.value)}
              placeholder="Напишите комментарий..."
              required
            />
            <Button type="submit" disabled={!newComment.trim()}>
              Опубликовать комментарий
            </Button>
          </Form>
          {comments.map((comment) => (
            <CommentItem
              key={comment.id}
              comment={comment}
              showAdminActions={userIsAdmin}
              currentUserId={user?.id}
              onMarkToxic={handleMarkCommentToxic}
              onAppeal={handleAppealComment}
              onReport={handleReportComment}
            />
          ))}
          <Pagination
            currentPage={commentsPage}
            totalPages={commentsTotalPages}
            onPageChange={setCommentsPage}
          />
        </CommentsSection>
      </ProjectDetailContainer>
    </Container>
  );
}

function CommentItem({
  comment,
  showAdminActions,
  currentUserId,
  onMarkToxic,
  onAppeal,
  onReport
}: {
  comment: Comment;
  showAdminActions: boolean;
  currentUserId?: string;
  onMarkToxic: (commentId: string) => void;
  onAppeal: (commentId: string) => void;
  onReport: (commentId: string) => void;
}) {
  const [actionBusy, setActionBusy] = useState(false);
  const isAuthor = currentUserId === comment.authorId;
  const isToxic = comment.moderationStatus === "Toxic";
  const canAppeal = isToxic && isAuthor && comment.moderatedBy === "AI" && (!comment.appealStatus || comment.appealStatus === "None");
  const isAppealing = comment.appealStatus === "Pending";
  const appealRejected = comment.appealStatus === "Rejected";
  const appealApproved = comment.appealStatus === "Approved";
  const canReport = Boolean(currentUserId) && !isAuthor && !isToxic;

  const runAction = async (action: (commentId: string) => void | Promise<void>) => {
    setActionBusy(true);
    try {
      await action(comment.id);
    } finally {
      setActionBusy(false);
    }
  };

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
          <ProfilePic src={userService.getProfilePicturePath(comment.authorUsername)} alt={comment.authorUsername} />
        )}
        <CommentUserLink to={`/users/${comment.authorUsername}`}>
          {comment.authorUsername}
        </CommentUserLink>
        {comment.userScore && <SmallStars>{Array.from({ length: comment.userScore }, () => '★').join('')}</SmallStars>}
        <CommentDate>{formatDate(comment.createdAt)}</CommentDate>
      </CommentHeader>
      {isToxic && isAuthor && (
        <ToxicBadge>
          ⚠️ Комментарий помечен как токсичный. Отправьте апелляцию, если считаете это ошибкой.
        </ToxicBadge>
      )}
      <CommentText>{comment.content}</CommentText>
      {(showAdminActions || canAppeal || canReport || isAppealing || appealRejected || appealApproved) && (
        <CommentActions>
          {showAdminActions && (
            <ActionButton type="button" danger disabled={actionBusy} onClick={() => runAction(onMarkToxic)}>
              Пометить токсичным
            </ActionButton>
          )}
          {canAppeal && (
            <ActionButton type="button" disabled={actionBusy} onClick={() => runAction(onAppeal)}>
              Отправить апелляцию
            </ActionButton>
          )}
          {isAppealing && <CommentStatus>⏳ Апелляция на рассмотрении</CommentStatus>}
          {appealRejected && <CommentStatus status="rejected">❌ Апелляция отклонена</CommentStatus>}
          {appealApproved && <CommentStatus status="approved">✅ Апелляция одобрена</CommentStatus>}
          {canReport && (
            <ActionButton type="button" disabled={actionBusy} onClick={() => runAction(onReport)}>
              Пожаловаться
            </ActionButton>
          )}
        </CommentActions>
      )}
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

const CommentActions = styled.div`
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: ${spacing.sm};
  margin-top: ${spacing.sm};
`;

const ActionButton = styled.button<{ danger?: boolean }>`
  background: ${props => props.danger ? "#dc3545" : colors.primary};
  color: ${colors.white};
  border: none;
  padding: ${spacing.xs} ${spacing.sm};
  border-radius: ${borderRadius.sm};
  cursor: pointer;
  font-weight: ${fonts.weight.semibold};
  transition: background ${transitions.base};

  &:hover {
    background: ${props => props.danger ? "#c82333" : colors.primaryDark};
  }

  &:disabled {
    background: ${colors.muted};
    cursor: not-allowed;
  }
`;

const ToxicBadge = styled.div`
  background: #fff3cd;
  color: #856404;
  border: 1px solid #ffc107;
  border-radius: ${borderRadius.sm};
  padding: ${spacing.sm} ${spacing.md};
  margin-bottom: ${spacing.sm};
  font-size: ${fonts.size.sm};
  font-weight: ${fonts.weight.medium};
`;

const CommentStatus = styled.span<{ status?: string }>`
  color: ${props => props.status === 'approved' ? '#155724' : props.status === 'rejected' ? '#721c24' : colors.textSecondary};
  font-size: ${fonts.size.sm};
  font-weight: ${props => props.status ? fonts.weight.semibold : fonts.weight.normal};
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

const ProjectDetail = () => (
  <ProjectProvider>
    <ProjectDetailContent />
  </ProjectProvider>
);

export default ProjectDetail;