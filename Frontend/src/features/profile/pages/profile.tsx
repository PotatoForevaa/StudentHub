import { useEffect, useContext } from "react";
import { useParams } from "react-router-dom";
import { Container, CardsContainer } from "../../../shared/components/Container";
import { AuthContext } from "../../auth/context/AuthContext";
import styled from "styled-components";
import { ProjectCard } from "../../projects/components/ProjectCard";
import { Pagination } from "../../projects/components/Pagination";
import userService from "../../../shared/services/userService";
import { useProfile } from "../hooks/useProfile";
import { useUserProjects } from "../hooks/useUserProjects";
import { useUserActivities } from "../hooks/useUserActivities";
import { ActivityList } from "../components/ActivityList";

const Picture = styled.img`
        width: 300px;
        height: 300px;
        margin: 25px 0 0 25px;
        border-radius: 15%;
        object-fit: cover;
        border: 2px solid #ddd;
    `;

const ProfileHeader = styled.div`
  display: flex;
  gap: 20px;
  align-items: center;
  margin-bottom: 20px;
`;

const UserInfo = styled.div`
  display: flex;
  flex-direction: column;
  gap: 10px;
`;

const Username = styled.h1`
  margin: 0;
`;

const FullName = styled.p`
  margin: 0;
`;

const StatsSection = styled.div`
  margin-bottom: 20px;
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 8px;
`;



const SectionTitle = styled.h2`
  margin: 20px 0 10px 0;
`;



const EditButton = styled.button`
  margin-top: 10px;
  padding: 10px 15px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
`;

const HiddenFileInput = styled.input`
  display: none;
`;

export const Profile = () => {
  const { username } = useParams<{ username: string }>();
  const { user: currentUser, loading: authLoading } = useContext(AuthContext);

  const { user, picture, loading: profileLoading, error, pictureLoading } = useProfile(username);
  const { projects, paginatedProjects, loading: projectsLoading, error: projectsError, currentPage, totalPages, setCurrentPage } = useUserProjects(
    user?.id || '',
    user?.username || ''
  );
  const { activities, loading: activitiesLoading, error: activitiesError } = useUserActivities(user?.username || '');

  const isOwnProfile = currentUser?.username === user?.username;



  const calculateStats = () => {
    if (!projects.length) return { count: 0, avgRating: 0 };
    const count = projects.length;
    const avgRating = projects.reduce((sum, p) => sum + (p.averageRating || 0), 0) / count;
    return { count, avgRating };
  };

  const stats = calculateStats();

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      userService.uploadProfilePicture(file).then(() => {
        window.location.reload();
      });
    }
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  if (authLoading || profileLoading) {
    return (
      <Container>
        <SectionTitle>Профиль</SectionTitle>
        <p>Загрузка профиля...</p>
      </Container>
    );
  }

  if (error || !user) {
    return (
      <Container>
        <SectionTitle>Профиль</SectionTitle>
        <p>{error || 'Пользователь не найден'}</p>
      </Container>
    );
  }

  return (
    <Container>
      <SectionTitle>Профиль</SectionTitle>
      <ProfileHeader>
        {picture && <Picture src={picture} alt="Профиль" />}
        <UserInfo>
          <Username>{user.username}</Username>
          <FullName>{user.fullName}</FullName>
          {isOwnProfile && (
            <div>
              <EditButton onClick={() => document.getElementById('file-input')?.click()}>
                Изменить фото профиля
              </EditButton>
              <HiddenFileInput
                id="file-input"
                type="file"
                accept="image/*"
                onChange={handleFileChange}
              />
            </div>
          )}
        </UserInfo>
      </ProfileHeader>

      <StatsSection>
        <h3>Статистика</h3>
        <p>Проекты: {stats.count}</p>
        <p>Средний рейтинг: {stats.avgRating.toFixed(1)}</p>
      </StatsSection>

      <SectionTitle>{isOwnProfile ? 'Мои проекты' : `Проекты пользователя ${user.username}`}</SectionTitle>
      {projectsLoading ? (
        <p>Загрузка проектов...</p>
      ) : projectsError ? (
        <p>{projectsError}</p>
      ) : paginatedProjects.length > 0 ? (
        <>
          <CardsContainer>
            {paginatedProjects.map(project => (
              <ProjectCard key={project.id} project={project} />
            ))}
          </CardsContainer>
          <Pagination
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={handlePageChange}
          />
        </>
      ) : (
        <p>Проектов пока нет.</p>
      )}

      <ActivityList
        activities={activities}
        loading={activitiesLoading}
        error={activitiesError}
      />
    </Container>
  );
};
