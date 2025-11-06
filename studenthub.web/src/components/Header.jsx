import { Link } from "react-router-dom";
import { styled } from "styled-components";

export const Header = () => {
  const Header = styled.header`
    background: #2C5AA0;
    padding: 0 20px;
  `;

  const Nav = styled.nav`
    display: flex;
    justify-content: space-between;
    align-items: center;
    max-width: 1200px;
    margin: 0 auto;
    height: 60px;
  `;

  const Ul = styled.ul`
    list-style-type: none;
    display: flex;
    gap: 20px;
    margin: 0;
    padding: 0;
  `;

  const NavLink = styled(Link)`
    color: white;
    text-decoration: none;
    padding: 8px 16px;
    border-radius: 4px;
    transition: background 0.3s;

    &:hover {
      background: #2C5AA0;
    }
  `;

  return (
    <Header>
      <Nav>
        <Ul>
          <li>
            <NavLink to="projects">Проекты</NavLink>
          </li>
          <li>
            <NavLink to="users">Пользователи</NavLink>
          </li>
        </Ul>

        <Ul>
          <li>
            <NavLink to="login">Вход</NavLink>
          </li>
          <li>
            <NavLink to="registration">Регистрация</NavLink>
          </li>
          <li>
            <NavLink to="auth">auth</NavLink>
          </li>
        </Ul>
      </Nav>
    </Header>
  );
};
