import { useCallback, useContext, useEffect, useMemo, useState } from "react";
import { Link, Navigate } from "react-router-dom";
import styled from "styled-components";
import { Container } from "../../../shared/components/Container";
import { Pagination } from "../../projects/components/Pagination";
import { adminService, type AdminProject } from "../services/adminService";
import type { PaginatedResponse, User } from "../../../shared/types";
import type { Comment } from "../../projects/types";
import { AuthContext } from "../../auth/context/AuthContext";
import { canModerate, isAdmin } from "../../../shared/utils/roles";
import { colors, fonts, spacing, borderRadius, transitions } from "../../../shared/styles/tokens";

const PAGE_SIZE = 10;
const ASSIGNABLE_ROLES = ["User", "Teacher", "Moderator"];

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

const Toolbar = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: ${spacing.sm};
`;

const Input = styled.input`
  min-width: 260px;
  padding: ${spacing.sm};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.sm};
`;

const Select = styled.select`
  padding: ${spacing.sm};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.sm};
  background: ${colors.white};
`;

const Table = styled.table`
  width: 100%;
  border-collapse: collapse;
  background: ${colors.surface};
  border: 1px solid ${colors.accentBorderLight};
`;

const Th = styled.th`
  text-align: left;
  padding: ${spacing.sm};
  border-bottom: 1px solid ${colors.accentBorder};
  color: ${colors.textSecondary};
  font-size: ${fonts.size.sm};
`;

const Td = styled.td`
  padding: ${spacing.sm};
  border-bottom: 1px solid ${colors.accentBorderLight};
  vertical-align: top;
`;

const Button = styled.button<{ danger?: boolean }>`
  border: none;
  border-radius: ${borderRadius.sm};
  padding: ${spacing.sm} ${spacing.md};
  color: ${colors.white};
  background: ${props => props.danger ? "#dc3545" : colors.primary};
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

type Tab = "users" | "projects" | "comments";

const emptyPage = <T,>(): PaginatedResponse<T> => ({
  items: [],
  page: 1,
  pageSize: PAGE_SIZE,
  totalCount: 0,
  totalPages: 0,
});

const WebAdmin = () => {
  const { user, loading } = useContext(AuthContext);
  const admin = isAdmin(user);
  const moderator = canModerate(user);
  const [tab, setTab] = useState<Tab>(admin ? "users" : "comments");
  const [search, setSearch] = useState("");
  const [role, setRole] = useState("");
  const [queue, setQueue] = useState<"reported" | "ai-toxic">("reported");
  const [page, setPage] = useState(1);
  const [users, setUsers] = useState<PaginatedResponse<User>>(emptyPage<User>());
  const [projects, setProjects] = useState<PaginatedResponse<AdminProject>>(emptyPage<AdminProject>());
  const [comments, setComments] = useState<PaginatedResponse<Comment>>(emptyPage<Comment>());
  const [busy, setBusy] = useState(false);

  const availableTabs = useMemo<Tab[]>(() => admin ? ["users", "projects", "comments"] : ["comments"], [admin]);

  useEffect(() => {
    if (!availableTabs.includes(tab)) setTab(availableTabs[0]);
  }, [availableTabs, tab]);

  useEffect(() => {
    setPage(1);
  }, [tab, search, role, queue]);

  const loadData = useCallback(async () => {
    if (!moderator) return;
    setBusy(true);
    try {
      if (tab === "users" && admin) {
        const result = await adminService.getUsers({ search, role, page, pageSize: PAGE_SIZE });
        if (result.isSuccess && result.data) setUsers(result.data);
      } else if (tab === "projects" && admin) {
        const result = await adminService.getProjects({ search, page, pageSize: PAGE_SIZE });
        if (result.isSuccess && result.data) setProjects(result.data);
      } else if (tab === "comments") {
        const result = await adminService.getModerationComments({ queue, page, pageSize: PAGE_SIZE });
        if (result.isSuccess && result.data) setComments(result.data);
      }
    } finally {
      setBusy(false);
    }
  }, [admin, moderator, page, queue, role, search, tab]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  if (loading) return <Container>Loading...</Container>;
  if (!moderator) return <Navigate to="/" replace />;

  const deleteUser = async (id: string) => {
    if (!window.confirm("Delete this user from the system?")) return;
    await adminService.deleteUser(id);
    await loadData();
  };

  const deleteProject = async (id: string) => {
    if (!window.confirm("Delete this project from the system?")) return;
    await adminService.deleteProject(id);
    await loadData();
  };

  const changeRole = async (id: string, nextRole: string) => {
    await adminService.updateUserRole(id, nextRole);
    await loadData();
  };

  const approveComment = async (id: string) => {
    await adminService.approveComment(id);
    await loadData();
  };

  const markToxic = async (id: string) => {
    await adminService.markCommentToxic(id);
    await loadData();
  };

  const totalPages = tab === "users" ? users.totalPages : tab === "projects" ? projects.totalPages : comments.totalPages;

  return (
    <Container>
      <Page>
        <Header>
          <Title>WebAdmin</Title>
          {busy && <Muted>Loading...</Muted>}
        </Header>

        <Tabs>
          {availableTabs.map(item => (
            <TabButton key={item} active={tab === item} onClick={() => setTab(item)}>
              {item === "users" ? "Users" : item === "projects" ? "Projects" : "Comments"}
            </TabButton>
          ))}
        </Tabs>

        {tab !== "comments" && admin && (
          <Toolbar>
            <Input value={search} onChange={e => setSearch(e.target.value)} placeholder="Search..." />
            {tab === "users" && (
              <Select value={role} onChange={e => setRole(e.target.value)}>
                <option value="">All roles</option>
                {["Admin", ...ASSIGNABLE_ROLES].map(item => <option key={item} value={item}>{item}</option>)}
              </Select>
            )}
          </Toolbar>
        )}

        {tab === "comments" && (
          <Toolbar>
            <Select value={queue} onChange={e => setQueue(e.target.value as "reported" | "ai-toxic")}>
              <option value="reported">Reported</option>
              <option value="ai-toxic">AI toxic</option>
            </Select>
          </Toolbar>
        )}

        {tab === "users" && admin && (
          users.items.length > 0 ? (
            <Table>
              <thead>
                <tr>
                  <Th>Username</Th>
                  <Th>Full name</Th>
                  <Th>Roles</Th>
                  <Th>Assignable role</Th>
                  <Th>Actions</Th>
                </tr>
              </thead>
              <tbody>
                {users.items.map(item => {
                  const currentRole = ASSIGNABLE_ROLES.find(r => item.roles?.includes(r)) ?? "User";
                  return (
                    <tr key={item.id}>
                      <Td>{item.username}</Td>
                      <Td>{item.fullName}</Td>
                      <Td>{item.roles?.join(", ") || "User"}</Td>
                      <Td>
                        <Select value={currentRole} onChange={e => changeRole(item.id, e.target.value)}>
                          {ASSIGNABLE_ROLES.map(r => <option key={r} value={r}>{r}</option>)}
                        </Select>
                      </Td>
                      <Td><Button danger onClick={() => deleteUser(item.id)}>Delete</Button></Td>
                    </tr>
                  );
                })}
              </tbody>
            </Table>
          ) : <Empty>No users found.</Empty>
        )}

        {tab === "projects" && admin && (
          projects.items.length > 0 ? (
            <Table>
              <thead>
                <tr>
                  <Th>Name</Th>
                  <Th>Author</Th>
                  <Th>Created</Th>
                  <Th>Actions</Th>
                </tr>
              </thead>
              <tbody>
                {projects.items.map(item => (
                  <tr key={item.id}>
                    <Td><Link to={`/projects/${item.id}`}>{item.name}</Link></Td>
                    <Td>{item.authorName} ({item.authorUsername})</Td>
                    <Td>{new Date(item.createdAt).toLocaleDateString()}</Td>
                    <Td><Button danger onClick={() => deleteProject(item.id)}>Delete</Button></Td>
                  </tr>
                ))}
              </tbody>
            </Table>
          ) : <Empty>No projects found.</Empty>
        )}

        {tab === "comments" && (
          comments.items.length > 0 ? comments.items.map(item => (
            <CommentCard key={item.id}>
              <div>
                <strong>{item.authorUsername}</strong>{" "}
                <Muted>
                  on {item.projectId ? <Link to={`/projects/${item.projectId}`}>{item.projectName || "project"}</Link> : item.projectName}
                </Muted>
              </div>
              <div>{item.content}</div>
              <Muted>
                Status: {item.moderationStatus || "Unknown"} / {item.moderatedBy || "Unknown"} · Reports: {item.reportCount ?? 0}
              </Muted>
              <ButtonRow>
                <Button onClick={() => approveComment(item.id)}>Approve</Button>
                <Button danger onClick={() => markToxic(item.id)}>Mark toxic</Button>
              </ButtonRow>
            </CommentCard>
          )) : <Empty>No comments found.</Empty>
        )}

        <Pagination currentPage={page} totalPages={totalPages} onPageChange={setPage} />
      </Page>
    </Container>
  );
};

export default WebAdmin;
