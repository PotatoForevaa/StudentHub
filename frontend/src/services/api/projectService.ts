import api from "./base";

export const projectService = {
    getProjects: () => 
        api.get('/api/projects'),

    getProject: (id: string) => 
        api.get(`/api/projects/${id}`),

    addProject: (project: FormData) =>
        api.post('/api/projects/create', project)
};