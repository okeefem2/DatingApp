export class Pagination {
  public constructor(
    public currentPage?: number,
    public totalPages?: number,
    public itemsPerPage?: number,
    public totalItems?: number,
  ) {}
}

export class PaginatedResult<T> {
  public constructor(
    public result?: T,
    public pagination?: Pagination,
  ) {}
}
