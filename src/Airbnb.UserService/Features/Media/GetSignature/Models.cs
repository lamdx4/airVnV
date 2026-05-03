using Airbnb.Infrastructure.Media;

namespace Airbnb.UserService.Features.Media.GetSignature;

public record Request(string Folder);

public record Response(SignatureResponse Signature);
