import { useContext, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Container } from "../../../shared/components/Container";
import { AuthContext } from "../../auth/context/AuthContext";
import styled from "styled-components";
import { projectService } from "../../projects/services/projectService";
import { ProjectCard } from "../../projects/components/ProjectCard";
import userService from "../../../shared/services/userService";
import type { Project } from "../../projects/types";
import type { User } from "../../../shared/types";

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

const ProjectsList = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
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
  const { user, picture } = useContext(AuthContext);
  const { username } = useParams<{ username: string }>();
  const [targetUser, setTargetUser] = useState<User | null>(null);
  const [targetPicture, setTargetPicture] = useState<string | null>(null);
  const [userProjects, setUserProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [profileLoading, setProfileLoading] = useState(false);

  const isOwnProfile = !username;
  const currentUser = isOwnProfile ? user : targetUser;

  useEffect(() => {
    if (username) {
      if (user?.username === username) {
        setTargetUser(null);
        setProfileLoading(false);
      } else {
        setProfileLoading(true);
        setTargetUser(null);
        userService.getAllUsers().then(res => {
          if (res?.isSuccess && res.data) {
            const foundUser = res.data.find(u => u.username === username);
            setTargetUser(foundUser || null);
          } else {
            console.error('Failed to fetch user data:', res?.errors);
            setTargetUser(null);
          }
        })
        .catch(error => {
          console.error('Failed to fetch user data:', error);
          setTargetUser(null);
        })
        .finally(() => {
          setProfileLoading(false);
        });
      }
    } else {
      setTargetUser(null);
      setProfileLoading(false);
    }
  }, [username, user]);

  useEffect(() => {
    if (currentUser) {
      const fetchUserProjects = async () => {
        try {
          setLoading(true);
          const res = await projectService.getProjectsByUser(currentUser.id);
          if (res?.isSuccess && res.data) {
            setUserProjects(res.data);
          } else {
            console.warn('Failed to fetch user projects via API, falling back to filtering', res?.errors);
            const allRes = await projectService.getProjects();
            if (allRes?.isSuccess && allRes.data) {
              const filtered = allRes.data.filter(p => p.author === currentUser.username);
              setUserProjects(filtered);
            }
          }
        } catch (error) {
          console.error('Failed to fetch projects', error);
          try {
            const fallbackRes = await projectService.getProjects();
            if (fallbackRes?.isSuccess && fallbackRes.data) {
              const filtered = fallbackRes.data.filter(p => p.author === currentUser.username);
              setUserProjects(filtered);
            }
          } catch (fallbackError) {
            console.error('Fallback project fetch also failed', fallbackError);
            setUserProjects([]);
          }
        } finally {
          setLoading(false);
        }
      };

      fetchUserProjects();
    }
  }, [currentUser]);



  useEffect(() => {
    if (currentUser && !isOwnProfile) {
      userService.getProfilePicture(currentUser.username)
        .then(response => {
          if (response.ok) {
            response.blob().then(blob => {
              const url = URL.createObjectURL(blob);
              setTargetPicture(url);
            });
          }
        })
        .catch(error => {
          console.error('Failed to fetch profile picture', error);
        });
    } else {
      setTargetPicture(null);
    }
  }, [currentUser, isOwnProfile]);

  const calculateStats = () => {
    if (!userProjects.length) return { count: 0, avgRating: 0 };
    const count = userProjects.length;
    const avgRating = userProjects.reduce((sum, p) => sum + (p.averageRating || 0), 0) / count;
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

  const displayPicture = isOwnProfile ? picture : targetPicture;

  return (
    <Container>
      <SectionTitle>Профиль</SectionTitle>
      {profileLoading ? (
        <p>Загрузка профиля...</p>
      ) : (
        <>
          <ProfileHeader>
            { displayPicture && <Picture src={ displayPicture } alt="Профиль" /> }
            <UserInfo>
              { currentUser && (
                <>
                  <Username>{currentUser.username}</Username>
                  <FullName>{currentUser.fullName}</FullName>
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
                </>
              ) }
            </UserInfo>
          </ProfileHeader>

          <StatsSection>
            <h3>Статистика</h3>
            <p>Проекты: {stats.count}</p>
            <p>Средний рейтинг: {stats.avgRating.toFixed(1)}</p>
          </StatsSection>

          <SectionTitle>{isOwnProfile ? 'Мои проекты' : `Проекты пользователя ${currentUser?.username}`}</SectionTitle>
          {loading ? (
            <p>Загрузка проектов...</p>
          ) : userProjects.length ? (
            <ProjectsList>
              {userProjects.map(project => (
                <ProjectCard key={project.id} project={project} />
              ))}
            </ProjectsList>
          ) : (
            <p>Проектов пока нет.</p>
          )}


        </>
      )}
    </Container>
  );
};
