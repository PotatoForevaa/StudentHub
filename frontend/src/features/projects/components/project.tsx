import styled from "styled-components";
import { Container } from "../../../shared/components/Container";
import { useContext, useEffect, useState } from "react";
import { ProjectContext } from "../../../shared/context/ProjectContext";

const ProjectContainer = styled.div`
  background: #3500d3;
  width: 500px;
  height: 100px;
`;

export const Project = () => {
  const { projects } = useContext(ProjectContext);
  const [loading, setLoading] = useState(false);

  <Container>
    { projects.forEach(element => {
        
    }); }
  </Container>;
};
