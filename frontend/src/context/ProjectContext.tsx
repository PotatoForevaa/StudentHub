import { createContext, useEffect, useState, type ReactNode } from "react";
import { projectService } from "../services/api/projectService";
import type { Project } from "../types/Project";

export type ProjectContextType = {
    project: Project | null,
    projects: Project[] | null,
    getProject: (id: string) => Promise<void>;
    getProjects: () => Promise<void>;
};

export const ProjectContext = createContext<ProjectContextType>({
    project: null,
    projects: [],
    getProject: async (_id: string) => {},
    getProjects: async () => {}
});

export const ProjectProvider = ({ children }: { children: ReactNode }) => {
  const [project, setProject] = useState<Project | null>(null);
  const [projects, setProjects] = useState<Project[]>([]);

  useEffect(() => {
    getProjects();
  }, [])

  const getProject = async (id: string) => {
    try {
      const res = await projectService.getProject(id);
      if (res.isSuccess && res.data) {
        setProject(res.data as Project);
      } else {
        setProject(null);
      }
    } catch {
      setProject(null);
    }
  };

  const getProjects = async () => {
    try {
        const res = await projectService.getProjects();
        // debug: log the full response so we can see why projects may be missing
        // (visible in browser console when this code runs)
        // eslint-disable-next-line no-console
        console.debug('projectService.getProjects response:', res);
        if (res && res.isSuccess && res.data) {
          setProjects(res.data as Project[]);
        } else {
          // show empty array and surface an error to console for troubleshooting
          // eslint-disable-next-line no-console
          console.warn('Failed to load projects, ApiResponse:', res);
          setProjects([]);
        }
    } catch (err) {
        // eslint-disable-next-line no-console
        console.error('Error fetching projects:', err);
        setProjects([]);
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
