// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------
namespace DataDictionary.Tests.Translations
{
  public partial class VariableConverter
  {
      private void Initialise()
      {
          AddVariable("NID_PACKET", ConvertInt.INSTANCE);
          AddVariable("NID_VBCMK", ConvertInt.INSTANCE);
          AddVariable("Q_DIR", ConvertInt.INSTANCE);
          AddVariable("L_PACKET", ConvertInt.INSTANCE);
          AddVariable("M_VERSION", ConvertInt.INSTANCE);
          AddVariable("Q_SCALE", ConvertInt.INSTANCE);
          AddVariable("D_VALIDNV", ConvertInt.INSTANCE);
          AddVariable("NID_C", ConvertInt.INSTANCE);
          AddVariable("N_ITER", ConvertInt.INSTANCE);
          AddVariable("V_NVSHUNT", ConvertInt.INSTANCE);
          AddVariable("V_NVSTFF", ConvertInt.INSTANCE);
          AddVariable("V_NVONSIGHT", ConvertInt.INSTANCE);
          AddVariable("V_NVLIMSUPERV", ConvertInt.INSTANCE);
          AddVariable("V_NVUNFIT", ConvertInt.INSTANCE);
          AddVariable("V_NVREL", ConvertInt.INSTANCE);
          AddVariable("D_NVROLL", ConvertInt.INSTANCE);
          AddVariable("Q_NVSBTSMPERM", ConvertInt.INSTANCE);
          AddVariable("Q_NVEMRRLS", ConvertInt.INSTANCE);
          AddVariable("Q_NVGUIPERM", ConvertInt.INSTANCE);
          AddVariable("Q_NVSBFBPERM", ConvertInt.INSTANCE);
          AddVariable("Q_NVINHSMICPERM", ConvertInt.INSTANCE);
          AddVariable("V_NVALLOWOVTRP", ConvertInt.INSTANCE);
          AddVariable("V_NVSUPOVTRP", ConvertInt.INSTANCE);
          AddVariable("D_NVOVTRP", ConvertInt.INSTANCE);
          AddVariable("T_NVOVTRP", ConvertInt.INSTANCE);
          AddVariable("D_NVPOTRP", ConvertInt.INSTANCE);
          AddVariable("M_NVCONTACT", ConvertInt.INSTANCE);
          AddVariable("T_NVCONTACT", ConvertInt.INSTANCE);
          AddVariable("M_NVDERUN", ConvertInt.INSTANCE);
          AddVariable("D_NVSTFF", ConvertInt.INSTANCE);
          AddVariable("Q_NVDRIVER_ADHES", ConvertInt.INSTANCE);
          AddVariable("A_NVMAXREDADH1", ConvertInt.INSTANCE);
          AddVariable("A_NVMAXREDADH2", ConvertInt.INSTANCE);
          AddVariable("A_NVMAXREDADH3", ConvertInt.INSTANCE);
          AddVariable("Q_NVLOCACC", ConvertInt.INSTANCE);
          AddVariable("M_NVAVADH", ConvertInt.INSTANCE);
          AddVariable("M_NVEBCL", ConvertInt.INSTANCE);
          AddVariable("Q_NVKINT", ConvertInt.INSTANCE);
          AddVariable("Q_NVKVINTSET", ConvertInt.INSTANCE);
          AddVariable("A_NVP12", ConvertInt.INSTANCE);
          AddVariable("A_NVP23", ConvertInt.INSTANCE);
          AddVariable("V_NVKVINT", ConvertInt.INSTANCE);
          AddVariable("M_NVKVINT", ConvertInt.INSTANCE);
          AddVariable("L_NVKRINT", ConvertInt.INSTANCE);
          AddVariable("M_NVKRINT", ConvertInt.INSTANCE);
          AddVariable("M_NVKTINT", ConvertInt.INSTANCE);
          AddVariable("D_LINK", ConvertInt.INSTANCE);
          AddVariable("Q_NEWCOUNTRY", ConvertInt.INSTANCE);
          AddVariable("NID_BG", ConvertInt.INSTANCE);
          AddVariable("Q_LINKORIENTATION", ConvertInt.INSTANCE);
          AddVariable("Q_LINKREACTION", ConvertInt.INSTANCE);
          AddVariable("Q_LOCACC", ConvertInt.INSTANCE);
          AddVariable("Q_VBCO", ConvertInt.INSTANCE);
          AddVariable("T_VBC", ConvertInt.INSTANCE);
          AddVariable("V_MAIN", ConvertInt.INSTANCE);
          AddVariable("V_LOA", ConvertInt.INSTANCE);
          AddVariable("T_LOA", ConvertInt.INSTANCE);
          AddVariable("L_SECTION", ConvertInt.INSTANCE);
          AddVariable("Q_SECTIONTIMER", ConvertInt.INSTANCE);
          AddVariable("T_SECTIONTIMER", ConvertInt.INSTANCE);
          AddVariable("D_SECTIONTIMERSTOPLOC", ConvertInt.INSTANCE);
          AddVariable("L_ENDSECTION", ConvertInt.INSTANCE);
          AddVariable("Q_ENDTIMER", ConvertInt.INSTANCE);
          AddVariable("T_ENDTIMER", ConvertInt.INSTANCE);
          AddVariable("D_ENDTIMERSTARTLOC", ConvertInt.INSTANCE);
          AddVariable("Q_DANGERPOINT", ConvertInt.INSTANCE);
          AddVariable("D_DP", ConvertInt.INSTANCE);
          AddVariable("V_RELEASEDP", ConvertInt.INSTANCE);
          AddVariable("Q_OVERLAP", ConvertInt.INSTANCE);
          AddVariable("D_STARTOL", ConvertInt.INSTANCE);
          AddVariable("T_OL", ConvertInt.INSTANCE);
          AddVariable("D_OL", ConvertInt.INSTANCE);
          AddVariable("V_RELEASEOL", ConvertInt.INSTANCE);
          AddVariable("D_SR", ConvertInt.INSTANCE);
          AddVariable("D_GRADIENT", ConvertInt.INSTANCE);
          AddVariable("Q_GDIR", ConvertInt.INSTANCE);
          AddVariable("G_A", ConvertInt.INSTANCE);
          AddVariable("D_STATIC", ConvertInt.INSTANCE);
          AddVariable("V_STATIC", ConvertInt.INSTANCE);
          AddVariable("Q_FRONT", ConvertInt.INSTANCE);
          AddVariable("Q_DIFF", ConvertInt.INSTANCE);
          AddVariable("NC_CDDIFF", ConvertInt.INSTANCE);
          AddVariable("NC_DIFF", ConvertInt.INSTANCE);
          AddVariable("V_DIFF", ConvertInt.INSTANCE);
          AddVariable("D_TRACTION", ConvertInt.INSTANCE);
          AddVariable("M_VOLTAGE", ConvertInt.INSTANCE);
          AddVariable("NID_CTRACTION", ConvertInt.INSTANCE);
          AddVariable("D_CURRENT", ConvertInt.INSTANCE);
          AddVariable("M_CURRENT", ConvertInt.INSTANCE);
          AddVariable("D_LEVELTR", ConvertInt.INSTANCE);
          AddVariable("M_LEVELTR", ConvertInt.INSTANCE);
          AddVariable("NID_NTC", ConvertInt.INSTANCE);
          AddVariable("L_ACKLEVELTR", ConvertInt.INSTANCE);
          AddVariable("Q_RBC", ConvertInt.INSTANCE);
          AddVariable("NID_RBC", ConvertInt.INSTANCE);
          AddVariable("NID_RADIO", ConvertHexTrailingFF.INSTANCE);
          AddVariable("Q_SLEEPSESSION", ConvertInt.INSTANCE);
          AddVariable("NID_XUSER", ConvertInt.INSTANCE);
          AddVariable("NID_MN", ConvertString.INSTANCE);
          AddVariable("Q_TRACKINIT", ConvertInt.INSTANCE);
          AddVariable("D_AXLELOAD", ConvertInt.INSTANCE);
          AddVariable("L_AXLELOAD", ConvertInt.INSTANCE);
          AddVariable("M_AXLELOADCAT", ConvertInt.INSTANCE);
          AddVariable("V_AXLELOAD", ConvertInt.INSTANCE);
          AddVariable("D_TRACKINIT", ConvertInt.INSTANCE);
          AddVariable("D_PBD", ConvertInt.INSTANCE);
          AddVariable("G_PBDSR", ConvertInt.INSTANCE);
          AddVariable("Q_PBDSR", ConvertInt.INSTANCE);
          AddVariable("D_PBDSR", ConvertInt.INSTANCE);
          AddVariable("L_PBDSR", ConvertInt.INSTANCE);
          AddVariable("T_MAR", ConvertInt.INSTANCE);
          AddVariable("T_TIMEOUTRQST", ConvertInt.INSTANCE);
          AddVariable("T_CYCRQST", ConvertInt.INSTANCE);
          AddVariable("T_CYCLOC", ConvertInt.INSTANCE);
          AddVariable("D_CYCLOC", ConvertInt.INSTANCE);
          AddVariable("M_LOC", ConvertInt.INSTANCE);
          AddVariable("D_LOC", ConvertInt.INSTANCE);
          AddVariable("Q_LGTLOC", ConvertInt.INSTANCE);
          AddVariable("NID_TSR", ConvertInt.INSTANCE);
          AddVariable("D_TSR", ConvertInt.INSTANCE);
          AddVariable("L_TSR", ConvertInt.INSTANCE);
          AddVariable("V_TSR", ConvertInt.INSTANCE);
          AddVariable("D_TRACKCOND", ConvertInt.INSTANCE);
          AddVariable("L_TRACKCOND", ConvertInt.INSTANCE);
          AddVariable("M_TRACKCOND", ConvertInt.INSTANCE);
          AddVariable("M_PLATFORM", ConvertInt.INSTANCE);
          AddVariable("Q_PLATFORM", ConvertInt.INSTANCE);
          AddVariable("D_SUITABILITY", ConvertInt.INSTANCE);
          AddVariable("Q_SUITABILITY", ConvertInt.INSTANCE);
          AddVariable("M_LINEGAUGE", ConvertInt.INSTANCE);
          AddVariable("D_ADHESION", ConvertInt.INSTANCE);
          AddVariable("L_ADHESION", ConvertInt.INSTANCE);
          AddVariable("M_ADHESION", ConvertInt.INSTANCE);
          AddVariable("Q_TEXTCLASS", ConvertInt.INSTANCE);
          AddVariable("Q_TEXTDISPLAY", ConvertInt.INSTANCE);
          AddVariable("D_TEXTDISPLAY", ConvertInt.INSTANCE);
          AddVariable("M_MODETEXTDISPLAY", ConvertInt.INSTANCE);
          AddVariable("M_LEVELTEXTDISPLAY", ConvertInt.INSTANCE);
          AddVariable("L_TEXTDISPLAY", ConvertInt.INSTANCE);
          AddVariable("T_TEXTDISPLAY", ConvertInt.INSTANCE);
          AddVariable("Q_TEXTCONFIRM", ConvertInt.INSTANCE);
          AddVariable("Q_CONFTEXTDISPLAY", ConvertInt.INSTANCE);
          AddVariable("Q_TEXTREPORT", ConvertString.INSTANCE);
          AddVariable("NID_TEXTMESSAGE", ConvertInt.INSTANCE);
          AddVariable("L_TEXT", ConvertInt.INSTANCE);
          AddVariable("X_TEXT", ConvertString.INSTANCE);
          AddVariable("Q_TEXT", ConvertInt.INSTANCE);
          AddVariable("D_POSOFF", ConvertInt.INSTANCE);
          AddVariable("Q_MPOSITION", ConvertInt.INSTANCE);
          AddVariable("M_POSITION", ConvertInt.INSTANCE);
          AddVariable("D_MAMODE", ConvertInt.INSTANCE);
          AddVariable("M_MAMODE", ConvertInt.INSTANCE);
          AddVariable("V_MAMODE", ConvertInt.INSTANCE);
          AddVariable("L_MAMODE", ConvertInt.INSTANCE);
          AddVariable("L_ACKMAMODE", ConvertInt.INSTANCE);
          AddVariable("Q_MAMODE", ConvertInt.INSTANCE);
          AddVariable("NID_LX", ConvertInt.INSTANCE);
          AddVariable("D_LX", ConvertInt.INSTANCE);
          AddVariable("L_LX", ConvertInt.INSTANCE);
          AddVariable("Q_LXSTATUS", ConvertInt.INSTANCE);
          AddVariable("V_LX", ConvertInt.INSTANCE);
          AddVariable("Q_STOPLX", ConvertInt.INSTANCE);
          AddVariable("L_STOPLX", ConvertInt.INSTANCE);
          AddVariable("D_RBCTR", ConvertInt.INSTANCE);
          AddVariable("Q_ASPECT", ConvertInt.INSTANCE);
          AddVariable("Q_RIU", ConvertInt.INSTANCE);
          AddVariable("NID_RIU", ConvertInt.INSTANCE);
          AddVariable("D_INFILL", ConvertInt.INSTANCE);
          AddVariable("NID_LOOP", ConvertInt.INSTANCE);
          AddVariable("D_LOOP", ConvertInt.INSTANCE);
          AddVariable("L_LOOP", ConvertInt.INSTANCE);
          AddVariable("Q_LOOPDIR", ConvertInt.INSTANCE);
          AddVariable("Q_SSCODE", ConvertInt.INSTANCE);
          AddVariable("Q_SRSTOP", ConvertInt.INSTANCE);
          AddVariable("D_STARTREVERSE", ConvertInt.INSTANCE);
          AddVariable("L_REVERSEAREA", ConvertInt.INSTANCE);
          AddVariable("D_REVERSE", ConvertInt.INSTANCE);
          AddVariable("V_REVERSE", ConvertInt.INSTANCE);
          AddVariable("NID_OPERATIONAL", ConvertString.INSTANCE);
          AddVariable("G_TSR", ConvertInt.INSTANCE);
          AddVariable("Q_LSSMA", ConvertInt.INSTANCE);
          AddVariable("T_LSSMA", ConvertInt.INSTANCE);
          AddVariable("NID_LRBG", ConvertInt.INSTANCE);
          AddVariable("D_LRBG", ConvertInt.INSTANCE);
          AddVariable("Q_DIRLRBG", ConvertInt.INSTANCE);
          AddVariable("Q_DLRBG", ConvertInt.INSTANCE);
          AddVariable("L_DOUBTOVER", ConvertInt.INSTANCE);
          AddVariable("L_DOUBTUNDER", ConvertInt.INSTANCE);
          AddVariable("Q_LENGTH", ConvertInt.INSTANCE);
          AddVariable("L_TRAININT", ConvertInt.INSTANCE);
          AddVariable("V_TRAIN", ConvertInt.INSTANCE);
          AddVariable("Q_DIRTRAIN", ConvertInt.INSTANCE);
          AddVariable("M_MODE", ConvertInt.INSTANCE);
          AddVariable("M_LEVEL", ConvertInt.INSTANCE);
          AddVariable("NID_PRVLRBG", ConvertInt.INSTANCE);
          AddVariable("M_ERROR", ConvertInt.INSTANCE);
          AddVariable("NID_LTRBG", ConvertInt.INSTANCE);
          AddVariable("NC_CDTRAIN", ConvertInt.INSTANCE);
          AddVariable("NC_TRAIN", ConvertInt.INSTANCE);
          AddVariable("L_TRAIN", ConvertInt.INSTANCE);
          AddVariable("V_MAXTRAIN", ConvertInt.INSTANCE);
          AddVariable("M_LOADINGGAUGE", ConvertInt.INSTANCE);
          AddVariable("M_AIRTIGHT", ConvertInt.INSTANCE);
          AddVariable("N_AXLE", ConvertInt.INSTANCE);
          AddVariable("Q_UPDOWN", ConvertInt.INSTANCE);
          AddVariable("Q_MEDIA", ConvertInt.INSTANCE);
          AddVariable("N_PIG", ConvertInt.INSTANCE);
          AddVariable("N_TOTAL", ConvertInt.INSTANCE);
          AddVariable("M_DUP", ConvertInt.INSTANCE);
          AddVariable("M_MCOUNT", ConvertInt.INSTANCE);
          AddVariable("Q_LINK", ConvertInt.INSTANCE);
          AddVariable("NID_MESSAGE", ConvertInt.INSTANCE);
          AddVariable("L_MESSAGE", ConvertInt.INSTANCE);
          AddVariable("T_TRAIN", ConvertInt.INSTANCE);
          AddVariable("NID_ENGINE", ConvertInt.INSTANCE);
          AddVariable("Q_MARQSTREASON", ConvertInt.INSTANCE);
          AddVariable("NID_EM", ConvertInt.INSTANCE);
          AddVariable("Q_EMERGENCYSTOP", ConvertInt.INSTANCE);
          AddVariable("Q_INFILL", ConvertInt.INSTANCE);
          AddVariable("Q_STATUS", ConvertInt.INSTANCE);
          AddVariable("M_ACK", ConvertInt.INSTANCE);
          AddVariable("D_REF", ConvertInt.INSTANCE);
          AddVariable("D_EMERGENCYSTOP", ConvertInt.INSTANCE);
          AddVariable("D_TAFDISPLAY", ConvertInt.INSTANCE);
          AddVariable("L_TAFDISPLAY", ConvertInt.INSTANCE);
          AddVariable("Q_ORIENTATION", ConvertInt.INSTANCE);
          AddVariable("Q_NVSRBKTRG", ConvertInt.INSTANCE);
          AddVariable("M_TRACTION", ConvertInt.INSTANCE);
          AddVariable("M_AXLELOAD", ConvertInt.INSTANCE);
          AddVariable("M_AXLELOADC", ConvertInt.INSTANCE);
          AddVariable("M_TRACKCONDBC", ConvertInt.INSTANCE);
          AddVariable("Q_TRACKDEL", ConvertInt.INSTANCE);
          AddVariable("Priority", ConvertInt.INSTANCE);
        }
    }
}
