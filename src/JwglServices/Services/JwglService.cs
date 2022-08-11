using Grpc.Core;

namespace JwglServices.Services
{
    public class JwglService : Jwgler.JwglerBase
    {
        public override Task<GetSemesterResponse> GetSemester(GetSemesterRequest request, ServerCallContext context)
        {
            return base.GetSemester(request, context);
        }
    }
}
