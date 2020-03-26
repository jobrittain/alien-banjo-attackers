using ABAFS.Sprites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ABAFS
{
    /// <summary>
    /// Manages saving and loading data to a specified XML file.
    /// </summary>
    public class SaveManager
    {
        public string FileName;
        public bool Saving;

        Playfield _playfield;
        XmlReader _reader;
        XmlWriter _writer;
        XmlWriterSettings _writerSettings;
        string _exeDirectory;

        public SaveManager(string fileName, Playfield playfield)
        {
            _playfield = playfield;
            _exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            _writerSettings = new XmlWriterSettings();
            _writerSettings.Indent = true;
            _writerSettings.NewLineOnAttributes = true;
            FileName = fileName;
        }

        /// <summary>
        /// Saves the current session to the XML file.
        /// </summary>
        /// <param name="userSave"></param>
        public void Save(bool userSave)
        {
            Saving = true;
            _writer = XmlWriter.Create(_exeDirectory + FileName, _writerSettings);
            _writer.WriteStartDocument();
            _writer.WriteStartElement("savegame");

            _writer.WriteElementString("highScore", _playfield.HighScore.ToString());
            _writer.WriteElementString("userSave", userSave.ToString().ToLower());
            _writer.WriteElementString("IDCount", _playfield.IDCount.ToString());

            // Player
            _writer.WriteStartElement("player");

            _writer.WriteElementString("ID", _playfield.Player.ID.ToString());
            _writer.WriteElementString("score", _playfield.Player.Score.Value.ToString());
            _writer.WriteElementString("lives", _playfield.Player.Lives.ToString());

            _writer.WriteStartElement("location");
            _writer.WriteElementString("x", _playfield.Player.Location.X.ToString());
            _writer.WriteElementString("y", _playfield.Player.Location.Y.ToString());
            _writer.WriteEndElement();

            _writer.WriteEndElement();

            // Notes
            _writer.WriteStartElement("notes");
            _writer.WriteElementString("count", _playfield.Notes.Count.ToString());
            foreach (Note note in _playfield.Notes)
            {
                _writer.WriteStartElement("note");

                _writer.WriteElementString("ID", note.ID.ToString());
                _writer.WriteElementString("parentID", note.ParentID.ToString());

                _writer.WriteStartElement("location");
                _writer.WriteElementString("x", note.Location.X.ToString());
                _writer.WriteElementString("y", note.Location.Y.ToString());
                _writer.WriteEndElement();

                _writer.WriteEndElement();
            }
            _writer.WriteEndElement();

            // Banjos
            _writer.WriteStartElement("banjos");
            _writer.WriteElementString("count", _playfield.Banjos.Count.ToString());
            foreach (Banjo banjo in _playfield.Banjos)
            {
                _writer.WriteStartElement("banjo");

                _writer.WriteElementString("ID", banjo.ID.ToString());
                _writer.WriteElementString("age", banjo.AgeInMilliseconds.ToString());
                _writer.WriteElementString("hitPoints", banjo.HitPoints.ToString());
                _writer.WriteElementString("type", banjo.Type.ToString());

                _writer.WriteStartElement("location");
                _writer.WriteElementString("x", banjo.Location.X.ToString());
                _writer.WriteElementString("y", banjo.Location.Y.ToString());
                _writer.WriteEndElement();

                _writer.WriteEndElement();
            }
            _writer.WriteEndElement();

            _writer.WriteEndElement();
            _writer.WriteEndDocument();
            _writer.Close();
            Saving = false;
        }
        /// <summary>
        /// Loads the XML data to the current session.
        /// </summary>
        /// <returns>Returns false if the save file indicates an automatic save (after gameover) or  cannot be read</returns>
        public bool Load()
        {
            if (System.IO.File.Exists(_exeDirectory + FileName) == true)
            {
                _reader = XmlReader.Create(_exeDirectory + FileName);
                try
                {
                    int count;
                    int ID;
                    int parentID;
                    int ageInMillliSecs;
                    int hitPoints;
                    Banjo.BanjoType type;
                    float x;
                    float y;

                    while (_reader.Read() == true)
                    {
                        if (_reader.IsStartElement())
                        {
                            switch (_reader.Name)
                            {
                                case "savegame":
                                    _reader.ReadToFollowing("highScore");
                                    _playfield.HighScore.Value = _reader.ReadElementContentAsInt();
                                    _reader.ReadToFollowing("userSave");
                                    if (_reader.ReadElementContentAsBoolean() == false)
                                    {
                                        _reader.Close();
                                        return false;
                                    }
                                    _reader.ReadToFollowing("IDCount");
                                    _playfield.IDCount = _reader.ReadElementContentAsInt();
                                    break;

                                case "player":
                                    _reader.ReadToFollowing("ID");
                                    _playfield.Player.ID = _reader.ReadElementContentAsInt();
                                    _reader.ReadToFollowing("score");
                                    _playfield.Player.Score.Value = _reader.ReadElementContentAsInt();
                                    _reader.ReadToFollowing("lives");
                                    _playfield.Player.Lives = _reader.ReadElementContentAsInt();
                                    _reader.ReadToFollowing("location");
                                    _reader.ReadToFollowing("x");
                                    _playfield.Player.Location.X = _reader.ReadElementContentAsFloat();
                                    _reader.ReadToFollowing("y");
                                    _playfield.Player.Location.Y = _reader.ReadElementContentAsFloat();
                                    break;

                                case "notes":
                                    _reader.ReadToFollowing("count");
                                    count = _reader.ReadElementContentAsInt();
                                    while (count > 0)
                                    {
                                        _reader.ReadToFollowing("ID");
                                        ID = _reader.ReadElementContentAsInt();
                                        _reader.ReadToFollowing("parentID");
                                        parentID = _reader.ReadElementContentAsInt();
                                        _reader.ReadToFollowing("x");
                                        x = _reader.ReadElementContentAsFloat();
                                        _reader.ReadToFollowing("y");
                                        y = _reader.ReadElementContentAsFloat();

                                        _playfield.LoadNote(ID, parentID, x, y);

                                        count--;
                                    }
                                    break;

                                case "banjos":
                                    _reader.ReadToFollowing("count");
                                    count = _reader.ReadElementContentAsInt();
                                    while (count > 0)
                                    {
                                        _reader.ReadToFollowing("ID");
                                        ID = _reader.ReadElementContentAsInt();
                                        _reader.ReadToFollowing("age");
                                        ageInMillliSecs = _reader.ReadElementContentAsInt();
                                        _reader.ReadToFollowing("hitPoints");
                                        hitPoints = _reader.ReadElementContentAsInt();
                                        _reader.ReadToFollowing("type");
                                        type = (Banjo.BanjoType)Enum.Parse(typeof(Banjo.BanjoType), _reader.ReadElementContentAsString());
                                        _reader.ReadToFollowing("x");
                                        x = _reader.ReadElementContentAsFloat();
                                        _reader.ReadToFollowing("y");
                                        y = _reader.ReadElementContentAsFloat();

                                        _playfield.LoadBanjo(ID, type, x, y, ageInMillliSecs);

                                        count--;
                                    }
                                    break;
                            }
                        }
                    }
                    _reader.Close();
                }
                catch (Exception e)
                {
                    try
                    {
                        File.Delete(_exeDirectory + FileName);
                    }
                    catch
                    {
                        Debug.WriteLine("Failed to delete settings file " + FileName + "." + e.Message);
                    }

                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

    }
}
