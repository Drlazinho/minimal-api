using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // Arrange
        var adm = new Administrador();

        //Act
        adm.Email = "teste@teste.com";
        adm.Senha = "senha123";
        adm.Perfil = "adm";
        adm.Id = 1;

        //Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("senha123", adm.Senha);
        Assert.AreEqual("adm", adm.Perfil);
    }
}
