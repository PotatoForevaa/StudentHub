import { createContext, useState, type ReactNode } from "react";
import { projectService } from "../../services/api/projectService";
import type { Project } from "../types/Project";

export type ProjectContextType = {
    projects: Project[],
    getProject: (id: string) => Promise<void>;
    getProjects: () => Promise<void>;
};

export const ProjectContext = createContext<ProjectContextType>({
    projects: [],
    getProject: async () => {},
    getProjects: async () => {}
});

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [project, setProject] = useState(null);
  const [projects, setProjects] = useState(null);

  const getProject = async (id: string) => {
    try {
      const res = await projectService.getProject(id);
      setProject(res.data);
    } catch {
      setProject(null);
    }
  };

  const getProjects = async () => {
    try {
        const res = await projectService.getProjects();
        setProjects(res.data);
    } catch {
        setProjects(null);
    }
  }

  return (
    <ProjectContext.Provider
      value={{
        getProject,
        getProjects,
        project,
        projects
      }}
    >
      {children}
    </ProjectContext.Provider>
  );
};
