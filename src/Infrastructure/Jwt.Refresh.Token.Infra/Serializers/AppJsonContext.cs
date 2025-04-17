using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Forms;

namespace Jwt.Refresh.Token.Infra.AspNetCore.Serializers;

[JsonSerializable(typeof(Domain.DataTransferObjects.Token))]
[JsonSerializable(typeof(AntiforgeryToken))]
public partial class AppJsonContext : JsonSerializerContext
{
}