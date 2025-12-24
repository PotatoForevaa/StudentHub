import { NavLink, useNavigate } from "react-router-dom";
import { styled } from "styled-components";
import { AuthContext } from "../features/auth/context/AuthContext";
import { useContext } from "react";
import { colors, shadows, transitions, fonts } from "../shared/styles/tokens";

const StyledHeader = styled.header`
  background: ${colors.surface};
  box-shadow: ${shadows.header};
  position: sticky;
  top: 0;
  z-index: 1000;
  border-bottom: 1px solid ${colors.accentBorderLight};
`;

const Nav = styled.nav`
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 72px;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;

  @media (max-width: 720px) {
    padding: 0 12px;
  }
`;

const Ul = styled.ul`
  list-style-type: none;
  display: flex;
  margin: 0;
  padding: 0;
  height: 100%;
  gap: 6px;
`;

const Li = styled.li`
  height: 100%;
  display: flex;
  align-items: center;
`;

const NavLinkStyled = styled(NavLink)`
  color: ${colors.textPrimary};
  text-decoration: none;
  height: 100%;
  padding: 0 14px;
  display: flex;
  align-items: center;
  font-weight: ${fonts.weight.semibold};
  font-size: ${fonts.size.base};
  transition: color ${transitions.base}, transform ${transitions.base};
  border-radius: 6px;
  position: relative;
  white-space: nowrap;

  &:hover { color: ${colors.primaryDark}; transform: translateY(-2px) }
  &.active { color: ${colors.primary} }

  &::after {
    content: "";
    position: absolute;
    bottom: 8px;
    left: 50%;
    width: 0;
    height: 3px;
    background: linear-gradient(90deg, ${colors.primary}, ${colors.primaryDark});
    transition: width ${transitions.slow};
    transform: translateX(-50%);
    border-radius: 2px;
  }

  &:hover::after, &.active::after {
    width: 60%;
  }
`;

const Logo = styled(NavLink)`
  font-size: ${fonts.size['3xl']};
  font-weight: ${fonts.weight.bold};
  color: ${colors.textPrimary};
  margin-right: 20px;
  display: flex;
  align-items: center;
  text-decoration: none;
  cursor: pointer;
  transition: color ${transitions.base};

  &:hover {
    color: ${colors.primaryDark};
  }

  span { color: ${colors.primary}; margin-left: 6px }

  @media (max-width: 480px) { font-size: ${fonts.size.lg} }
`;

const LeftSection = styled.div`
  display: flex;
  align-items: center;
  height: 100%;
`;

const RightSection = styled.div`
  display: flex;
  align-items: center;
  height: 100%;
`;

const StyledP = styled.p`
  color: ${colors.textPrimary};
  margin: 0;
  line-height: 1.1;
`;

const StyledImg = styled.img`
  width: 44px;
  height: 44px;
  margin: 0 0 0 12px;
  border-radius: 10px;
  object-fit: cover;
  border: 1px solid ${colors.accentBorderLight};
`;

const LogoutButton = styled.button`
  background: transparent;
  border: 1px solid ${colors.accentBorderDark};
  border-radius: 8px;
  padding: 8px 16px;
  color: ${colors.textPrimary};
  font-weight: ${fonts.weight.semibold};
  font-size: ${fonts.size.sm};
  cursor: pointer;
  transition: all ${transitions.base};
  margin-left: 12px;

  &:hover {
    background: ${colors.primary};
    color: ${colors.white};
    border-color: ${colors.primary};
  }
`;

export const Header = () => {
  const { isAuthenticated, user, picture, logout } = useContext(AuthContext);
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };
  return (
    <StyledHeader>
      <Nav>
        <LeftSection>
          <Logo to="/leaderboard">StudentHub</Logo>
          <Ul>
            <Li>
              <NavLinkStyled
                to="projects"
                className={({ isActive }) => (isActive ? "active" : "")}
              >
                Проекты
              </NavLinkStyled>
            </Li>
            <Li>
              <NavLinkStyled
                to="users"
                className={({ isActive }) => (isActive ? "active" : "")}
              >
                Пользователи
              </NavLinkStyled>
            </Li>
          </Ul>
        </LeftSection>

        <RightSection>
          <Ul>
            {!isAuthenticated && (
              <>
                <Li>
                  <NavLinkStyled
                    to="login"
                    className={({ isActive }) => (isActive ? "active" : "")}
                  >
                    Вход
                  </NavLinkStyled>
                </Li>
                <Li>
                  <NavLinkStyled
                    to="registration"
                    className={({ isActive }) => (isActive ? "active" : "")}
                  >
                    Регистрация
                  </NavLinkStyled>
                </Li>
              </>
            )}
            {isAuthenticated && (
              <>
                <Li>
                  <NavLinkStyled
                    to={user?.username ? `/${user.username}` : "/"}
                    className={({ isActive }) => (isActive ? "active" : "")}
                  >
                    <StyledP>
                      {user?.fullName}
                      <br />
                      {user?.username}
                    </StyledP>
                    { picture && <StyledImg src={picture} />}
                  </NavLinkStyled>
                </Li>
                <Li>
                  <LogoutButton onClick={handleLogout}>
                    Выйти
                  </LogoutButton>
                </Li>
              </>
            )}
          </Ul>
        </RightSection>
      </Nav>
    </StyledHeader>
  );
};
