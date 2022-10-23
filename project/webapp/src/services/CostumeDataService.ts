import http from "@/http-common";

class CostumeDataService {
  getAll(): Promise<any> {
    return http.get("/costumes");
  }

  get(id: any): Promise<any> {
    return http.get(`/costumes/${id}`);
  }

  create(data: any): Promise<any> {
    return http.post("/costumes", data);
  }

  update(id: any, data: any): Promise<any> {
    return http.put(`/costumes/${id}`, data);
  }

  delete(id: any): Promise<any> {
    return http.delete(`/costumes/${id}`);
  }

  deleteAll(): Promise<any> {
    return http.delete(`/costumes`);
  }

  findByTitle(title: string): Promise<any> {
    return http.get(`/costumes/${title}`);
  }
}

export default new CostumeDataService();
