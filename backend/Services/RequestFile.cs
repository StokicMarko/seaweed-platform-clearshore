namespace Services;

///Tiny data holder that says "this file belongs to this request" :P
///An applier says which files its requests carr (returns list), the review service can check a file name and content without payload look (OC)

public record RequestFile(string StoredFileName, string OriginalFileName, string ContentType); //read onlyy properties, record =small class holding values