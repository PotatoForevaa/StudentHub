import { useCallback, useContext, useEffect, useState } from "react";
import { Link, Navigate } from "react-router-dom";
import styled from "styled-components";
import { Container } from "../../../shared/components/Container";
import { Pagination } from "../../projects/components/Pagination";
import { moderationService, type ModerationQueue } from "../services/moderationService";
import { MuteModal } from "../components/MuteModal";
import type { Comment } from "../../projects/types";
import type { PaginatedResponse } from "../../../shared/types";
import { AuthContext } from "../../auth/context/AuthContext";
import { canModerate } from "../../../shared/utils/roles";
import { colors, fonts, spacing, borderRadius, transitions } from "../../../shared/styles/tokens";

const PAGE_SIZE = 10;

type Queue = "reported" | "ai-toxic" | "all-toxic" | "appeals";

const Page = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: ${spacing.lg};
`;

const Header = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: ${spacing.md};
`;

const Title = styled.h1`
  margin: 0;
  color: ${colors.textPrimary};
  font-size: ${fonts.size["2xl"]};
`;

const Tabs = styled.div`
  display: flex;
  gap: ${spacing.sm};
  border-bottom: 1px solid ${colors.accentBorder};
`;

const TabButton = styled.button<{ active: boolean }>`
  border: none;
  border-bottom: 3px solid ${props => props.active ? colors.primary : "transparent"};
  background: transparent;
  color: ${props => props.active ? colors.primary : colors.textPrimary};
  padding: ${spacing.sm} ${spacing.md};
  font-weight: ${fonts.weight.semibold};
  cursor: pointer;
`;

const CommentCard = styled.div`
  border: 1px solid ${colors.accentBorderLight};
  background: ${colors.surface};
  border-radius: ${borderRadius.md};
  padding: ${spacing.md};
  display: flex;
  flex-direction: column;
  gap: ${spacing.sm};
`;

const Muted = styled.span`
  color: ${colors.textSecondary};
  font-size: ${fonts.size.sm};
`;

const Empty = styled.p`
  color: ${colors.textSecondary};
`;

const Button = styled.button<{ danger?: boolean; success?: boolean }>`
  border: none;
  border-radius: ${borderRadius.sm};
  padding: ${spacing.sm} ${spacing.md};
  color: ${colors.white};
  background: ${props => props.danger ? "#dc3545" : props.success ? "#28a745" : colors.primary};
  cursor: pointer;
  transition: filter ${transitions.base};
  &:hover { filter: brightness(0.95); }
  &:disabled { opacity: 0.6; cursor: not-allowed; }
`;

const ButtonRow = styled.div`
  display: flex;
  gap: ${spacing.sm};
  flex-wrap: wrap;
`;

const StatusBadge = styled.span<{ color: string }>`
  display: inline-block;
  padding: 2px 8px;
  border-radius: ${borderRadius.sm};
  font-size: ${fonts.size.xs};
  font-weight: ${fonts.weight.semibold};
  background: ${props => props.color}22;
  color: ${props => props.color};
  margin-right: ${spacing.xs};
`;

const Toolbar = styled.div`
  display: flex;
  gap: ${spacing.sm};
  flex-wrap: wrap;
`;

const Select = styled.select`
  padding: ${spacing.sm};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.sm};
  background: ${colors.white};
`;

const emptyPage = <T,>(): PaginatedResponse<T> => ({
  items: [],
  page: 1,
  pageSize: PAGE_SIZE,
  totalCount: 0,
  totalPages: 0,
});

const ModerationPage = () => {
  const { user, loading } = useContext(AuthContext);
  const canAccess = canModerate(user);
  const [queue, setQueue] = useState<Queue>("reported");
  const [page, setPage] = useState(1);
  const [comments, setComments] = useState<PaginatedResponse<Comment>>(emptyPage<Comment>());
  const [busy, setBusy] = useState(false);
  const [muteModal, setMuteModal] = useState<{ userId: string; userName: string } | null>(null);

  useEffect(() => { setPage(1); }, [queue]);

  const loadData = useCallback(async () => {
    if (!canAccess) return;
    setBusy(true);
    try {
      const queueParam: ModerationQueue = queue === "all-toxic" ? "moderator-toxic" : queue;
      const result = await moderationService.getComments({ 
        queue: queueParam, 
        page, 
        pageSize: PAGE_SIZE 
      });
      if (result.isSuccess && result.data) setComments(result.data);
    } finally {
      setBusy(false);
    }
  }, [canAccess, page, queue]);

  useEffect(() => { loadData(); }, [loadData]);

  if (loading) return <Container>Загрузка...</Container>;
  if (!canAccess) return <Navigate to="/" replace />;

  const approveComment = async (id: string) => {
    await moderationService.approveComment(id);
    await loadData();
  };

  const markToxic = async (id: string) => {
    await moderationService.markCommentToxic(id);
    await loadData();
  };

  const resolveAppeal = async (id: string, approved: boolean) => {
    await moderationService.resolveAppeal(id, approved);
    await loadData();
  };

  const getStatusColor = (status?: string) => {
    switch (status) {
      case "Approved": return "#28a745";
      case "Toxic": return "#dc3545";
      case "Pending": return "#ffc107";
      default: return colors.textSecondary;
    }
  };

  const getAppealColor = (status?: string) => {
    switch (status) {
      case "Pending": return "#ffc107";
      case "Approved": return "#28a745";
      case "Rejected": return "#dc3545";
      default: return colors.textSecondary;
    }
  };

  return (
    <Container>
      <Page>
        <Header>
          <Title>Модерация</Title>
          {busy && <Muted>Загрузка...</Muted>}
        </Header>

        <Tabs>
          {(["reported", "ai-toxic", "all-toxic", "appeals"] as Queue[]).map(item => (
            <TabButton key={item} active={queue === item} onClick={() => setQueue(item)}>
              {item === "reported" ? "Жалобы" : item === "ai-toxic" ? "Токсичные (AI)" : item === "all-toxic" ? "Все токсичные" : "Апелляции"}
            </TabButton>
          ))}
        </Tabs>

        {comments.items.length > 0 ? comments.items.map(comment => (
          <CommentCard key={comment.id}>
            <div>
              <Link to={`/users/${comment.authorUsername}`} style={{ fontWeight: fonts.weight.semibold, color: colors.primary }}>
                {comment.authorUsername}
              </Link>{" "}
              <Muted>{new Date(comment.createdAt).toLocaleDateString()}</Muted>
            </div>
            <div>{comment.content}</div>
            <div>
              {comment.projectId && (
                <Muted>
                  на <Link to={`/projects/${comment.projectId}`}>{comment.projectName || "проект"}</Link>
                </Muted>
              )}
            </div>
            <div>
              <StatusBadge color={getStatusColor(comment.moderationStatus)}>{comment.moderationStatus || "Неизвестно"}</StatusBadge>
              <StatusBadge color={colors.textSecondary}>{comment.moderatedBy || "Неизвестно"}</StatusBadge>
              {comment.reportCount !== undefined && comment.reportCount > 0 && (
                <StatusBadge color="#dc3545">Жалобы: {comment.reportCount}</StatusBadge>
              )}
              {comment.appealStatus && comment.appealStatus !== "None" && (
                <StatusBadge color={getAppealColor(comment.appealStatus)}>Апелляция: {comment.appealStatus}</StatusBadge>
              )}
            </div>
            <ButtonRow>
              <Button success onClick={() => approveComment(comment.id)}>Одобрить</Button>
              <Button danger onClick={() => markToxic(comment.id)}>Пометить токсичным</Button>
              {queue === "appeals" && comment.appealStatus === "Pending" && (
                <>
                  <Button success onClick={() => resolveAppeal(comment.id, true)}>Одобрить апелляцию</Button>
                  <Button danger onClick={() => resolveAppeal(comment.id, false)}>Отклонить апелляцию</Button>
                </>
              )}
              <Button onClick={() => setMuteModal({ userId: comment.authorId, userName: comment.authorUsername })}>
                Заглушить автора
              </Button>
            </ButtonRow>
          </CommentCard>
        )) : <Empty>Комментарии не найдены.</Empty>}

        <Pagination currentPage={page} totalPages={comments.totalPages} onPageChange={setPage} />
      </Page>

      {muteModal && (
        <MuteModal
          isOpen={true}
          onClose={() => setMuteModal(null)}
          userId={muteModal.userId}
          userName={muteModal.userName}
          onMuted={() => { setMuteModal(null); loadData(); }}
        />
      )}
    </Container>
  );
};

export default ModerationPage;
