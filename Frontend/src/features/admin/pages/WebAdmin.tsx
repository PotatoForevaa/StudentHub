import { useCallback, useContext, useEffect, useState } from "react";
import { Link, Navigate } from "react-router-dom";
import styled from "styled-components";
import { Container } from "../../../shared/components/Container";
import { Pagination } from "../../projects/components/Pagination";
import { adminService, type AdminProject } from "../services/adminService";
import type { PaginatedResponse, User } from "../../../shared/types";
import type { TagDto, CategoryDto, CriterionDto } from "../../projects/types";
import { AuthContext } from "../../auth/context/AuthContext";
import { isAdmin } from "../../../shared/utils/roles";
import { colors, fonts, spacing, borderRadius, transitions } from "../../../shared/styles/tokens";

const PAGE_SIZE = 10;
const ASSIGNABLE_ROLES = ["User", "Teacher"];

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
  flex-wrap: wrap;
`;

const TabButton = styled.button<{ active: boolean }>`
  border: none;
  border-bottom: 3px solid ${props => props.active ? colors.primary : "transparent"};
  background: transparent;
  color: ${props => props.active ? colors.primary : colors.textPrimary};
  padding: ${spacing.sm} ${spacing.md};
  font-size: ${fonts.size.sm};
  font-weight: ${fonts.weight.semibold};
  cursor: pointer;
`;

const Toolbar = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: ${spacing.sm};
  align-items: center;
`;

const Input = styled.input`
  min-width: 260px;
  padding: ${spacing.sm};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.sm};
  font-size: ${fonts.size.sm};
`;

const Select = styled.select`
  padding: ${spacing.sm};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.sm};
  background: ${colors.white};
  font-size: ${fonts.size.sm};
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
  font-size: ${fonts.size.sm};
`;

const Button = styled.button<{ danger?: boolean; success?: boolean }>`
  border: none;
  border-radius: ${borderRadius.sm};
  padding: ${spacing.sm} ${spacing.md};
  color: ${colors.white};
  background: ${props => props.danger ? "#dc3545" : props.success ? "#28a745" : colors.primary};
  cursor: pointer;
  font-size: ${fonts.size.sm};
  transition: filter ${transitions.base};
  &:hover { filter: brightness(0.95); }
  &:disabled { opacity: 0.6; cursor: not-allowed; }
`;

const Muted = styled.span`
  color: ${colors.textSecondary};
  font-size: ${fonts.size.sm};
`;

const Empty = styled.p`
  color: ${colors.textSecondary};
`;

const InlineForm = styled.form`
  display: flex;
  gap: ${spacing.sm};
  align-items: center;
  flex-wrap: wrap;
`;

const Card = styled.div`
  background: ${colors.surface};
  border: 1px solid ${colors.accentBorderLight};
  border-radius: ${borderRadius.md};
  padding: ${spacing.lg};
`;

const CardHeader = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: ${spacing.sm};
`;

const CardTitle = styled.h3`
  margin: 0;
  font-size: ${fonts.size.base};
  color: ${colors.textPrimary};
`;

const TagBadge = styled.span`
  display: inline-flex;
  align-items: center;
  gap: 4px;
  background: ${colors.gray100};
  padding: 2px 8px;
  border-radius: 12px;
  font-size: ${fonts.size.sm};
  margin: 2px;
`;

const RemoveButton = styled.button`
  border: none;
  background: transparent;
  color: ${colors.textSecondary};
  cursor: pointer;
  padding: 0;
  font-size: ${fonts.size.sm};
  line-height: 1;
`;

const TagsContainer = styled.div`
  margin-top: ${spacing.lg};
`;

const FiltersRow = styled.div`
  margin-top: ${spacing.lg};
`;

type Tab = "users" | "projects" | "categories" | "tags" | "criteria";

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
  const [tab, setTab] = useState<Tab>("users");
  const [search, setSearch] = useState("");
  const [role, setRole] = useState("");
  const [page, setPage] = useState(1);
  const [users, setUsers] = useState<PaginatedResponse<User>>(emptyPage<User>());
  const [projects, setProjects] = useState<PaginatedResponse<AdminProject>>(emptyPage<AdminProject>());
  const [busy, setBusy] = useState(false);

  // Categories state
  const [categoriesList, setCategoriesList] = useState<CategoryDto[]>([]);
  const [newCategoryName, setNewCategoryName] = useState("");

  // Tags state
  const [tags, setTags] = useState<TagDto[]>([]);
  const [newTagName, setNewTagName] = useState("");

  // Criteria state
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [criteria, setCriteria] = useState<CriterionDto[]>([]);
  const [selectedCategoryId, setSelectedCategoryId] = useState("");
  const [newCriterionName, setNewCriterionName] = useState("");

  useEffect(() => { setPage(1); }, [tab, search, role]);

  const loadData = useCallback(async () => {
    if (!admin) return;
    setBusy(true);
    try {
      if (tab === "users") {
        const result = await adminService.getUsers({ search, role, page, pageSize: PAGE_SIZE });
        if (result.isSuccess && result.data) setUsers(result.data);
      } else if (tab === "projects") {
        const result = await adminService.getProjects({ search, page, pageSize: PAGE_SIZE });
        if (result.isSuccess && result.data) setProjects(result.data);
      } else if (tab === "categories") {
        const result = await adminService.getAdminCategories();
        if (result.isSuccess && result.data) setCategoriesList(result.data);
      } else if (tab === "tags") {
        const result = await adminService.getAdminTags();
        if (result.isSuccess && result.data) setTags(result.data);
      } else if (tab === "criteria") {
        const [catResult, critResult] = await Promise.all([
          adminService.getAdminCategories(),
          adminService.getAdminCriteria(selectedCategoryId || undefined),
        ]);
        if (catResult.isSuccess && catResult.data) setCategories(catResult.data);
        if (critResult.isSuccess && critResult.data) setCriteria(critResult.data);
      }
    } finally { setBusy(false); }
  }, [admin, page, role, search, tab, selectedCategoryId]);

  useEffect(() => { loadData(); }, [loadData]);

  const createCategory = useCallback(async () => {
    if (!newCategoryName.trim()) return;
    const result = await adminService.createCategory(newCategoryName.trim());
    if (result.isSuccess) {
      setNewCategoryName("");
      loadData();
    }
  }, [newCategoryName, loadData]);

  const deleteCategory = useCallback(async (id: string) => {
    if (window.confirm("Удалить эту категорию?")) {
      await adminService.deleteCategory(id);
      loadData();
    }
  }, [loadData]);

  const createTag = useCallback(async () => {
    if (!newTagName.trim()) return;
    const result = await adminService.createTag(newTagName.trim());
    if (result.isSuccess) {
      setNewTagName("");
      loadData();
    }
  }, [newTagName, loadData]);

  const deleteTag = useCallback(async (id: string) => {
    if (window.confirm("Удалить этот тег?")) {
      await adminService.deleteTag(id);
      loadData();
    }
  }, [loadData]);

  const createCriterion = useCallback(async () => {
    if (!newCriterionName.trim() || !selectedCategoryId) return;
    const result = await adminService.createCriterion(newCriterionName.trim(), selectedCategoryId);
    if (result.isSuccess) {
      setNewCriterionName("");
      loadData();
    }
  }, [newCriterionName, selectedCategoryId, loadData]);

  const deleteCriterion = useCallback(async (id: string) => {
    if (window.confirm("Удалить этот критерий?")) {
      await adminService.deleteCriterion(id);
      loadData();
    }
  }, [loadData]);

  if (loading) return <Container>Загрузка...</Container>;
  if (!admin) return <Navigate to="/" replace />;

  const totalPages = tab === "users" ? users.totalPages : projects.totalPages;

  return (
    <Container>
      <Page>
        <Header>
          <Title>Администрирование</Title>
          {busy && <Muted>Загрузка...</Muted>}
        </Header>

        <Tabs>
          {(["users", "projects", "categories", "tags", "criteria"] as Tab[]).map(item => (
            <TabButton key={item} active={tab === item} onClick={() => setTab(item)}>
              {item === "users" ? "Пользователи" : item === "projects" ? "Проекты" : item === "categories" ? "Категории" : item === "tags" ? "Теги" : "Критерии"}
            </TabButton>
          ))}
        </Tabs>

        <Toolbar>
          {tab === "users" && (
            <>
              <Input value={search} onChange={e => setSearch(e.target.value)} placeholder="Поиск..." />
              <Select value={role} onChange={e => setRole(e.target.value)}>
                <option value="">Все роли</option>
                {["Admin", ...ASSIGNABLE_ROLES].map(item => <option key={item} value={item}>{item}</option>)}
              </Select>
            </>
          )}
          {tab === "projects" && (
            <Input value={search} onChange={e => setSearch(e.target.value)} placeholder="Поиск проектов..." />
          )}
        </Toolbar>

        {tab === "users" && (
          users.items.length > 0 ? (
            <Table>
              <thead>
                <tr>
                  <Th>Имя пользователя</Th>
                  <Th>Полное имя</Th>
                  <Th>Роли</Th>
                  <Th>Назначаемая роль</Th>
                  <Th>Действия</Th>
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
                        <Select value={currentRole} onChange={e => {
                          adminService.updateUserRole(item.id, e.target.value).then(() => loadData());
                        }}>
                          {ASSIGNABLE_ROLES.map(r => <option key={r} value={r}>{r}</option>)}
                        </Select>
                      </Td>
                      <Td>
                        <Button danger onClick={async () => {
                          if (window.confirm("Удалить этого пользователя?")) {
                            await adminService.deleteUser(item.id);
                            await loadData();
                          }
                        }}>Удалить</Button>
                      </Td>
                    </tr>
                  );
                })}
              </tbody>
            </Table>
          ) : <Empty>Пользователи не найдены.</Empty>
        )}

        {tab === "projects" && (
          projects.items.length > 0 ? (
            <Table>
              <thead>
                <tr>
                  <Th>Название</Th>
                  <Th>Автор</Th>
                  <Th>Создан</Th>
                  <Th>Действия</Th>
                </tr>
              </thead>
              <tbody>
                {projects.items.map(item => (
                  <tr key={item.id}>
                    <Td><Link to={`/projects/${item.id}`}>{item.name}</Link></Td>
                    <Td>{item.authorName} ({item.authorUsername})</Td>
                    <Td>{new Date(item.createdAt).toLocaleDateString()}</Td>
                    <Td>
                      <Button danger onClick={async () => {
                        if (window.confirm("Удалить этот проект?")) {
                          await adminService.deleteProject(item.id);
                          await loadData();
                        }
                        }}>Удалить</Button>
                    </Td>
                  </tr>
                ))}
              </tbody>
            </Table>
          ) : <Empty>Проекты не найдены.</Empty>
        )}

        {tab === "categories" && (
          <div>
            <Card>
              <CardHeader>
                <CardTitle>Создать категорию</CardTitle>
              </CardHeader>
              <InlineForm onSubmit={e => { e.preventDefault(); createCategory(); }}>
                <Input
                  value={newCategoryName}
                  onChange={e => setNewCategoryName(e.target.value)}
                  placeholder="Название категории"
                  maxLength={100}
                />
                <Button success type="submit" disabled={!newCategoryName.trim()}>Добавить категорию</Button>
              </InlineForm>
            </Card>
            <TagsContainer>
              {categoriesList.length > 0 ? (
                categoriesList.map(cat => (
                  <TagBadge key={cat.id}>
                    {cat.name}
                    <RemoveButton onClick={() => deleteCategory(cat.id)} title="Удалить категорию">&times;</RemoveButton>
                  </TagBadge>
                ))
              ) : <Empty>Категории ещё не созданы.</Empty>}
            </TagsContainer>
          </div>
        )}

        {tab === "tags" && (
          <div>
            <Card>
              <CardHeader>
                <CardTitle>Создать тег</CardTitle>
              </CardHeader>
              <InlineForm onSubmit={e => { e.preventDefault(); createTag(); }}>
                <Input
                  value={newTagName}
                  onChange={e => setNewTagName(e.target.value)}
                  placeholder="Название тега"
                  maxLength={50}
                />
                <Button success type="submit" disabled={!newTagName.trim()}>Добавить тег</Button>
              </InlineForm>
            </Card>
            <TagsContainer>
              {tags.length > 0 ? (
                tags.map(tag => (
                  <TagBadge key={tag.id}>
                    {tag.name}
                    <RemoveButton onClick={() => deleteTag(tag.id)} title="Удалить тег">&times;</RemoveButton>
                  </TagBadge>
                ))
              ) : <Empty>Теги ещё не созданы.</Empty>}
            </TagsContainer>
          </div>
        )}

        {tab === "criteria" && (
          <div>
            <Card>
              <CardHeader>
                <CardTitle>Создать критерий</CardTitle>
              </CardHeader>
              <InlineForm onSubmit={e => { e.preventDefault(); createCriterion(); }}>
                <Select value={selectedCategoryId} onChange={e => setSelectedCategoryId(e.target.value)} required>
                  <option value="">Выберите категорию...</option>
                  {categories.map(cat => <option key={cat.id} value={cat.id}>{cat.name}</option>)}
                </Select>
                <Input
                  value={newCriterionName}
                  onChange={e => setNewCriterionName(e.target.value)}
                  placeholder="Название критерия"
                  maxLength={100}
                />
                <Button success type="submit" disabled={!newCriterionName.trim() || !selectedCategoryId}>Добавить критерий</Button>
              </InlineForm>
            </Card>
            <FiltersRow>
              <Select value={selectedCategoryId} onChange={e => { setSelectedCategoryId(e.target.value); }}>
                <option value="">Фильтр по категории...</option>
                {categories.map(cat => <option key={cat.id} value={cat.id}>{cat.name}</option>)}
              </Select>
            </FiltersRow>
            <TagsContainer>
              {criteria.length > 0 ? (
                <Table>
                  <thead>
                    <tr>
                      <Th>Категория</Th>
                      <Th>Название</Th>
                      <Th>Действия</Th>
                    </tr>
                  </thead>
                  <tbody>
                    {criteria.map(c => (
                      <tr key={c.id}>
                        <Td>{c.categoryName}</Td>
                        <Td>{c.name}</Td>
                        <Td>
                          <Button danger onClick={() => deleteCriterion(c.id)}>Удалить</Button>
                        </Td>
                      </tr>
                    ))}
                  </tbody>
                </Table>
              ) : <Empty>Критерии не найдены для этой категории.</Empty>}
            </TagsContainer>
          </div>
        )}

        {(tab === "users" || tab === "projects") && (
          <Pagination currentPage={page} totalPages={totalPages} onPageChange={setPage} />
        )}
      </Page>
    </Container>
  );
};

export default WebAdmin;