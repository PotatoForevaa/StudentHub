import { Link } from 'react-router-dom';

export const Header = () => {

    return (
        <header>
            <nav>
                <ul>
                    <li><Link to="projects">Проекты</Link></li>
                    <li><Link to="users">Пользователи</Link></li>
                </ul>
                <ul>
                    <li><Link to="settings">Настройки</Link></li>
                    <li><Link to="account">Профиль</Link></li>
                </ul>
                <ul>
                    <li><Link to="login">Вход</Link></li>
                    <li><Link to="registration">Регистрация</Link></li>
                </ul>
            </nav>
        </header>
    );
}