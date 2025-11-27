import { useContext } from "react";
import { Container } from "../../../shared/components/Container";
import { AuthContext } from "../../../shared/context/AuthContext";
import styled from "styled-components";

const Picture = styled.img`
        width: 300px;
        height: 300px;
        margin: 25px 0 0 25px;
        border-radius: 15%
    `;

export const Profile = () => {
  const { picture } = useContext(AuthContext);
  return (
    <Container>
      { picture && <Picture src={ picture }/> }
    </Container>
  );
};
