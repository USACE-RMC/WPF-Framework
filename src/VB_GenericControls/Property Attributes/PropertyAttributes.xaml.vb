Imports System.ComponentModel
Public Class PropertyAttributes

#Region "Construction"

    ''' <summary>
    ''' Initialize an empty control.
    ''' </summary>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
    End Sub

#End Region

#Region "Methods"

    ''' <summary>
    ''' Sets the default attribute description to display. 
    ''' </summary>
    ''' <param name="displayName">The property display name.</param>
    ''' <param name="description">The property description.</param>
    Public Sub SetDefaultAttributes(displayName As String, description As String)
        Name.Text = displayName
        Me.Description.Text = description
    End Sub

    ''' <summary>
    ''' Gets the attributes for the specified class object. 
    ''' </summary>
    ''' <param name="classObject">The class object.</param>
    Public Sub GetClassAttributes(classObject As Object)
        ' Get the attributes for the class object.
        Dim attributes As AttributeCollection = TypeDescriptor.GetAttributes(classObject.GetType())
        ' Update the name and description text boxes.
        Name.Text = CType(attributes(GetType(DisplayNameAttribute)), DisplayNameAttribute).DisplayName.ToString()
        Description.Text = CType(attributes(GetType(DescriptionAttribute)), DescriptionAttribute).Description.ToString()
    End Sub

    ''' <summary>
    ''' Gets the attributes for the property from a specified class object. 
    ''' </summary>
    ''' <param name="propertyName">The property name.</param>
    ''' <param name="classObject">The class object.</param>
    Public Sub GetPropertyAttributes(propertyName As String, classObject As Object)
        ' Get property descriptors for the class.
        Dim propertyDescriptors As PropertyDescriptorCollection = TypeDescriptor.GetProperties(classObject.GetType())
        ' Get the attributes for property.
        Dim attributes As AttributeCollection = propertyDescriptors(propertyName).Attributes
        ' Update the name and description text boxes.
        Name.Text = CType(attributes(GetType(DisplayNameAttribute)), DisplayNameAttribute).DisplayName.ToString()
        Description.Text = CType(attributes(GetType(DescriptionAttribute)), DescriptionAttribute).Description.ToString()
    End Sub

#End Region

End Class
