using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace SampleCodeDom
{
    public class CodeDomHelper
    {
        #region private members

        private volatile static CodeDomHelper _instance = null;
        private CodeDomHelper() { }
        public static CodeDomHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CodeDomHelper();
                }
                return _instance;
            }
        }

        #endregion

        public CodeNamespace CreateNameSpace(string StrNameSpaceName, string[] ImportDllName, string comments)
        {
            try
            {
                if (StrNameSpaceName.Length < 0)
                    return null;
                CodeNamespace nameSpaceDeclaration = new CodeNamespace(StrNameSpaceName);
                if (ImportDllName != null)
                {
                    for (int m = 0; m < ImportDllName.Length; m++)
                    {
                        CodeNamespaceImport import = new CodeNamespaceImport(ImportDllName[m].ToString());
                        nameSpaceDeclaration.Imports.Add(import);
                    }
                }
                if (!string.IsNullOrEmpty(comments))
                {
                    nameSpaceDeclaration.Comments.Add(new CodeCommentStatement(comments, false));
                }
                return nameSpaceDeclaration;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CodeTypeDeclaration CreateClass(string StrClassName, MemberAttributes ClassAttributes, bool isPartial, string StrBaseName, string comments)
        {
            try
            {
                CodeTypeDeclaration classDeclaration = new CodeTypeDeclaration(StrClassName);
                classDeclaration.Attributes = ClassAttributes;// MemberAttributes.Public & MemberAttributes.FamilyAndAssembly;
                classDeclaration.IsPartial = isPartial;
                if (!string.IsNullOrEmpty(StrBaseName))
                {
                    classDeclaration.BaseTypes.Add(StrBaseName);
                }
                if (!string.IsNullOrEmpty(comments))
                {
                    classDeclaration.Comments.Add(new CodeCommentStatement(comments, false));
                }
                return classDeclaration;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CodeConstructor CreateConstructor(string StrClassName, MemberAttributes attributes, string content, string comments)
        {
            try
            {
                // Declare the constructor
                CodeConstructor constructor = new CodeConstructor();
                constructor.Attributes = attributes;
                constructor.Name = StrClassName;
                constructor.Statements.Add(new CodeSnippetStatement(content));
                if (!string.IsNullOrEmpty(comments))
                {
                    constructor.Comments.Add(new CodeCommentStatement(comments, false));
                }
                return constructor;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public CodeMemberField CreateField(string fieldName, string typeName, MemberAttributes attributes, CodeExpression initExpression, string comments)
        {
            try
            {
                CodeMemberField field = new CodeMemberField();
                field.Attributes = attributes;
                field.Name = fieldName;
                field.Type = new CodeTypeReference(typeName);
                if(initExpression != null)
                {
                    field.InitExpression = initExpression;
                }
                if (!string.IsNullOrEmpty(comments))
                {
                    field.Comments.Add(new CodeCommentStatement(comments));
                }
                return field;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public CodeMemberProperty CreateProperty(string propName, string typeName, MemberAttributes attributes, bool hasGet, CodeStatement getContent, bool hasSet, CodeStatement setContent, string comments)
        {
            try
            {
                CodeMemberProperty property = new CodeMemberProperty();
                property.Attributes = attributes;
                property.Name = propName;
                property.HasGet = hasGet;
                if(getContent != null)
                {
                    property.GetStatements.Add(getContent);
                }
                property.HasSet = hasSet;
                if(setContent != null)
                {
                    property.SetStatements.Add(setContent);
                }
                property.Type = new CodeTypeReference(typeName);
                if(!string.IsNullOrEmpty(comments))
                {
                    property.Comments.Add(new CodeCommentStatement(comments));
                }

                return property;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CodeMemberMethod CreateMethod(string StrMethodName, string StrMethodContent, MemberAttributes MethodAttributes, CodeParameterDeclarationExpression[] Parameter, CodeTypeReference ReturnType, string StrNote)
        {
            try
            {
                CodeMemberMethod methodDeclaration = new CodeMemberMethod();
                methodDeclaration.Name = StrMethodName;
                methodDeclaration.Attributes = MethodAttributes;
                if (Parameter != null && Parameter.Length > 0)
                    methodDeclaration.Parameters.AddRange(Parameter);
                methodDeclaration.ReturnType = ReturnType;
                CodeSnippetStatement codeContent = new CodeSnippetStatement(StrMethodContent);
                methodDeclaration.Statements.Add(codeContent);
                if (!string.IsNullOrEmpty(StrNote))
                    methodDeclaration.Comments.Add(new CodeCommentStatement(new CodeComment(StrNote, false)));
                return methodDeclaration;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool AssembleCode(ref CodeNamespace NameSpaceCode, CodeTypeDeclaration ClassCode, CodeConstructor constructor, CodeMemberField[] fields, CodeMemberProperty[] properties, CodeMemberMethod[] methods)
        {
            try
            {
                if (NameSpaceCode == null && ClassCode == null)
                {
                    return false;
                }
                NameSpaceCode.Types.Add(ClassCode);

                if(constructor != null)
                {
                    ClassCode.Members.Add(constructor);
                }
                if(fields != null && fields.Length > 0)
                {
                    ClassCode.Members.AddRange(fields);
                }
                if(properties != null && properties.Length > 0)
                {
                    ClassCode.Members.AddRange(properties);
                }
                if(methods != null && methods.Length > 0)
                {
                    ClassCode.Members.AddRange(methods);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GengerCode(CodeNamespace nspace)
        {
            StringBuilder sb = new StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter(sb);
            CodeGeneratorOptions geneOptions = new CodeGeneratorOptions();
            geneOptions.BlankLinesBetweenMembers = false;
            geneOptions.BracingStyle = "C";
            geneOptions.ElseOnClosing = true;
            geneOptions.IndentString = "    ";
            CodeDomProvider.GetCompilerInfo("C#").CreateProvider().GenerateCodeFromNamespace(nspace, sw, geneOptions);
            sw.Close();
            return sb.ToString();
        }

        public CompilerResults ExecuteDLL(CodeNamespace nspace, string filename)
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            unit.Namespaces.Add(nspace);
            CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
            CompilerParameters options = new CompilerParameters();//
            options.GenerateInMemory = false;
            options.IncludeDebugInformation = true;
            options.ReferencedAssemblies.Add("System.dll");
            options.OutputAssembly = filename;
            if (System.IO.Path.GetExtension(filename).ToLower() == ".exe")
            {
                options.GenerateExecutable = true;
            }
            else
            {
                options.GenerateExecutable = false;
            }
            return provider.CompileAssemblyFromDom(options, unit);
        }

        /// <summary>
        /// Generate CSharp source code from the compile unit.
        /// </summary>
        /// <param name="filename">Output file name</param>
        public void GenerateCSharpCode(CodeNamespace nspace, string fileName)
        {
            CodeCompileUnit targetUnit = new CodeCompileUnit();
            targetUnit.Namespaces.Add(nspace);
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter(fileName))
            {
                provider.GenerateCodeFromCompileUnit(
                    targetUnit, sourceWriter, options);
            }
        }

        public CodeNamespace GenerateContentForFormDesigner(string strNamespace, string strClassName, string initializeComponentContent, CodeMemberField[] fields, CodeMemberProperty[] properties)
        {
            CodeNamespace codeNamespace = CreateNameSpace(
                                strNamespace,
                                null,
                                null);
            CodeTypeDeclaration codeClass = CreateClass(
                                            strClassName,
                                            MemberAttributes.Family,
                                            true,
                                            null,
                                            null);
            CodeMemberField components = CreateField(
                                            "components",
                                            "System.ComponentModel.IContainer",
                                            MemberAttributes.Private,
                                            new CodeSnippetExpression("null"),
                                            "Required designer variable.");
            string disposeContent = string.Format("{0}if (disposing && (components != null)){1}components.Dispose();{2}base.Dispose(disposing);{3}",
                                        Constants.C_ThreeTab,
                                        Constants.C_NewLine + Constants.C_ThreeTab + "{" + Constants.C_NewLine + Constants.C_FourTab,
                                        Constants.C_NewLine + Constants.C_ThreeTab + "}" + Constants.C_NewLine + Constants.C_ThreeTab,
                                        Constants.C_NewLine);
            CodeMemberMethod dispose = CreateMethod(
                                        "Dispose",
                                        disposeContent,
                                        MemberAttributes.Family | MemberAttributes.Override,
                                        new CodeParameterDeclarationExpression[] { new CodeParameterDeclarationExpression(typeof(System.Boolean), "disposing") },
                                        new CodeTypeReference(typeof(void)),
                                        "Clean up any resources being used.");
            CodeMemberMethod initializeComponent = CreateMethod(
                                        "InitializeComponent",
                                        initializeComponentContent,
                                        MemberAttributes.Private,
                                        null,
                                        new CodeTypeReference(typeof(void)),
                                        "Required method for Designer support - do not modify\r\nthe contents of this method with the code editor.");
            if(fields == null || fields.Length == 0)
            {
                fields = new CodeMemberField[] { components };
            }
            else
            {
                CodeMemberField[] defaultFields = { components };
                CodeMemberField[] allFields = new CodeMemberField[fields.Length + defaultFields.Length];
                defaultFields.CopyTo(allFields, 0);
                fields.CopyTo(allFields, defaultFields.Length);
                fields = allFields;
            }
            AssembleCode(
                        ref codeNamespace,
                        codeClass,
                        null,
                        fields,
                        properties,
                        new CodeMemberMethod[] { dispose, initializeComponent });
            return codeNamespace;
        }
    }
}
