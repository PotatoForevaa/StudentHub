import { NavLink } from "react-router-dom";
import { styled } from "styled-components";
import { AuthContext } from "../shared/context/AuthContext";
import { useContext } from "react";

const StyledHeader = styled.header`
  background: linear-gradient(135deg, #190061 0%, #0c0032 100%);
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.3);
  position: sticky;
  top: 0;
  z-index: 1000;
`;

const Nav = styled.nav`
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin: 0 50px;
  height: 70px;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
`;

const Ul = styled.ul`
  list-style-type: none;
  display: flex;
  margin: 0;
  padding: 0;
  height: 100%;
  gap: 5px;
`;

const Li = styled.li`
  height: 100%;
  display: flex;
  align-items: center;
`;

const NavLinkStyled = styled(NavLink)`
  color: #ffffff;
  text-decoration: none;
  height: 100%;
  padding: 0 20px;
  display: flex;
  align-items: center;
  font-weight: 500;
  font-size: 16px;
  transition: all 0.3s ease;
  border-radius: 4px;
  position: relative;
  white-space: nowrap;

  &:hover {
    background: rgba(53, 0, 211, 0.2);
    color: #ffffff;
    transform: translateY(-1px);
  }

  &.active {
    background: rgba(53, 0, 211, 0.3);
    color: #ffffff;
  }

  &::after {
    content: "";
    position: absolute;
    bottom: 0;
    left: 50%;
    width: 0;
    height: 2px;
    background: #3500d3;
    transition: all 0.3s ease;
    transform: translateX(-50%);
  }

  &:hover::after {
    width: 80%;
  }
`;

const Logo = styled.div`
  font-size: 24px;
  font-weight: bold;
  color: #ffffff;
  margin-right: 30px;

  span {
    color: #3500d3;
  }
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

export const Header = () => {
  const { isAuthenticated } = useContext(AuthContext);
  return (
    <StyledHeader>
      <Nav>
        <LeftSection>
          <Logo>StudentHub</Logo>
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
          </Ul>
        </RightSection>
      </Nav>
    </StyledHeader>
  );
};
